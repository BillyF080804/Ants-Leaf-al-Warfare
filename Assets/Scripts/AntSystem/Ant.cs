using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Ant : MonoBehaviour {
	public enum PlayerList {
		Player1 = 0,
		Player2 = 1,
		Player3 = 2,
		Player4 = 3,
	}


	[Header("Ant Info")]
	public AntSO antInfo;

	public bool hasHadTurn = false;
	public PlayerList ownedPlayer;
	public Vector3 moveVector = Vector3.zero;
	public bool canJump = true;

	public List<EffectScript> effects;
	public bool hasLifeSteal;
	public float lifeStealPercent;
	public bool hasStatDrop;
	public List<EffectSO.StatDropType> statDrops;

	public bool isStepping = false;
	public bool isStoodBack = false;

	private int health;
	private Rigidbody rb;
	private Slider healthSlider;
	protected TurnManager turnManager;

	private bool canMove = true;

    private void Awake() {
		turnManager = FindFirstObjectByType<TurnManager>();
        rb = GetComponent<Rigidbody>();
		healthSlider = GetComponentInChildren<Slider>();
		health = antInfo.health;

		healthSlider.maxValue = health;
		healthSlider.value = health;
    }


	public void SetCanMove(bool _canMove) {
		canMove = _canMove;
	}
	public bool GetCanMove() {
		return canMove;
	}

    public void OnMove(InputValue value) {
		Vector2 movement = value.Get<Vector2>();
		moveVector = new Vector3(movement.x, 0, 0);
	}

	public void TakeDamage(int damage) {
		if (hasStatDrop) {
			if (statDrops.Contains(EffectSO.StatDropType.Defense)) {
				float percentage = 0;
				for(int i = 0; i < effects.Count; i++) {
					if (effects[i].effectInfo.statDropType == EffectSO.StatDropType.Defense) {
						percentage = effects[i].effectInfo.percentToDropBy;
						break;
					}
				}


				health -= (int)Mathf.Ceil(damage * percentage);
			}
		} else {
			health -= damage;
		}
		
		healthSlider.value = health;

		if (health <= 0) {
			OnDeath();
		}
	}

	public void OnDeath() {
		int playerNum = int.Parse(ownedPlayer.ToString().Last().ToString());
		Player player = turnManager.PlayerList.Where(x => x.playerInfo.playerNum == playerNum).First();

		if (turnManager.Gamemode.ToUpper().Contains("HVT")) {
			if (antInfo.IsQueen) {
				turnManager.GameOver();
			}
		}
		else if (turnManager.Gamemode.ToUpper().Contains("LAST") || turnManager.Gamemode.ToUpper().Equals("CLASSIC")) {
            if (antInfo.IsQueen == true && turnManager.Gamemode.ToUpper().Equals("CLASSIC")) {
                foreach (GameObject ant in player.AntList) {
                    ant.GetComponent<Ant>().TakeDamage(turnManager.DamageToDealOnQueenDeath);
                }
            }

            Dictionary<int, int> antsRemaining = new Dictionary<int, int>();

			for (int i = 0; i < turnManager.PlayerList.Count; i++) {
				antsRemaining.Add(turnManager.PlayerList[i].playerInfo.playerNum, turnManager.PlayerList[i].AntList.Count);

                if (turnManager.PlayerList[i].QueenAnt != null) {
					antsRemaining[turnManager.PlayerList[i].playerInfo.playerNum] += 1;
                }
            }

			if (antsRemaining.TryGetValue(playerNum, out int amount)) { 
				if (turnManager.PlayerList.Count == 1 && amount - 1 == 1) {
                    turnManager.GameOver();
                }
				else if (turnManager.PlayerList.Count == 2 && amount - 1 == 0) {
                    turnManager.GameOver();
                }
				else if (turnManager.PlayerList.Count == 3 || turnManager.PlayerList.Count == 4) {
					if (antsRemaining.Where(x => x.Value > 0).Count() == 2 && amount - 1 == 0) {
                        turnManager.GameOver();
                    }
				}
			}
		}

		DestroyAnt(antInfo.IsQueen, player);
    }

	private void DestroyAnt(bool isQueen, Player player) {
		if (isQueen) {
			player.RemoveQueen();
		}
		else {
			player.RemoveAnt(gameObject);
		}

		Destroy(gameObject);
	}

	public void HealDamage(int damageToHeal) {
		health += damageToHeal;
		healthSlider.value = health;
	}

	public int GetHealth() {
		return health;
	}

	public void SetHealth(int healthToSet) {
		health = healthToSet;
	}

	public void OnJump() {
		if (canJump && canMove) {
			Vector2 Force = new Vector2(0, antInfo.jumpHeight);
			rb.AddForce(Force, ForceMode.Impulse);
			canJump = false;
		}
	}

	public void StopMovement() {
		moveVector = Vector2.zero;
		rb.velocity = Vector3.zero;
	}

	public void SlowMovement(float slowMultiplier) {
		antInfo.moveSpeed *= slowMultiplier;
	}

	public void ResetMovement(float slowMultiplier) {
        antInfo.moveSpeed /= slowMultiplier;
    }

    private void Update() {
		if(canMove) {
			if (hasStatDrop) {
				if (statDrops.Contains(EffectSO.StatDropType.Speed)) {
					float percentage = 0;
					for (int i = 0; i < effects.Count; i++) {
						if (effects[i].effectInfo.statDropType == EffectSO.StatDropType.Speed) {
							percentage = effects[i].effectInfo.percentToDropBy;
							break;
						}
					}
					transform.Translate(antInfo.moveSpeed * Time.deltaTime * moveVector * percentage);

				}

			} else {
				transform.Translate(antInfo.moveSpeed * Time.deltaTime * moveVector);
			}
		}

		if(turnManager.CurrentAntTurn == this) {
			transform.position = new Vector3(transform.position.x, transform.position.y, 0);
		}
		
        CheckForNearbyAnts();
    }

	private void CheckForNearbyAnts() {
		List<Collider> ants = Physics.OverlapSphere(transform.position, 3.0f).Where(x => x.GetComponent<Ant>()).ToList();
		List<Ant> nearbyAnts = new List<Ant>();

		foreach (Collider collider in ants) {
			nearbyAnts.Add(collider.GetComponent<Ant>());
		}

		if (turnManager.CurrentAntTurn == this) {
            nearbyAnts.Remove(this);

            foreach (Ant ant in nearbyAnts) {
                if (ant.isStepping == false && ant.isStoodBack == false) {
                    ant.isStepping = true;
                    ant.MoveOutTheWay();
                }
            }
        }
		else {
			if (!nearbyAnts.Contains(turnManager.CurrentAntTurn) && isStepping == false && isStoodBack == true) {
				isStepping = true;
				ReturnToNormalPos();
			}
		}
	}

	public void MoveOutTheWay() {
        isStepping = true;
        StartCoroutine(StepCoroutine(-3.0f));
    }

	private void ReturnToNormalPos() {
		isStepping = true;
		StartCoroutine(StepCoroutine(0.0f));
	}

	private IEnumerator StepCoroutine(float targetZ) {
        float time = 0;
        Vector3 startingPos = transform.position;
        Vector3 targetPos = transform.position;
		targetPos.z = targetZ;

        //Lerp to actually move the UI
        while (time < 1.0f) {
            transform.position = LerpLibrary.ObjectLerp(startingPos, targetPos, LerpType.InOut, time);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
		isStepping = false;

		if (targetZ == 0.0f) {
			isStoodBack = false;
		}
		else {
            isStoodBack = true;
        }
    }

    private void OnCollisionEnter(Collision collision) {
		if(Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit ray, 3.0f)) {
			canJump = true;
		}
	}

	public bool canInteract;
	public Interactable interactable;
	private void OnTriggerEnter(Collider other) {
		Debug.Log("Entered");
		if (other.GetComponent<Interactable>() != null) {
			interactable = other.GetComponent<Interactable>();
			canInteract = true;
		}
	}

	private void OnTriggerExit(Collider other) {
		Debug.Log("Exited");
		if (other.GetComponent<Interactable>() != null) {
			interactable = null;
			canInteract = false;
		}
	}
	
	public void ApplyEffects() {
		if (effects.Count > 0) {
			for (int i = 0; i < effects.Count; i++) {
				effects[i].ApplyEffect(this);
			}
		}
	}
}
