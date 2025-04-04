using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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

	public bool isDrowning = false;
	public bool isCrushed = false;

	[SerializeField] private Animator animator;

	[Header("UI Settings")]
	[SerializeField] private FadeScript healthFadeScript;
	[SerializeField] private TMP_Text healthChangeText;
	[SerializeField] private TMP_Text healthText;
	[SerializeField] private GameObject healthTextBackground;

	[field : Header("Weapon Transform")]
	[field : SerializeField] public Transform WeaponTransform { get; private set; }

    private int health;
	private Rigidbody rb;
	private Player player;
	protected TurnManager turnManager;

	private bool canMove = true;

    private void Awake() {
		turnManager = FindFirstObjectByType<TurnManager>();
        rb = GetComponent<Rigidbody>();

        int playerNum = int.Parse(ownedPlayer.ToString().Last().ToString());
        player = turnManager.PlayerList.Where(x => x.playerInfo.playerNum == playerNum).First();

        TurnManager.onTurnEnded += FadeOutHealthUI;
		WeaponManager.onOpenWeaponsMenu += FadeInHealthUI;
        WeaponManager.onCloseWeaponsMenu += FadeOutHealthUI;

		health = antInfo.health;

		if (healthText != null) {
            healthText.text = health.ToString();
            healthText.transform.parent.GetComponent<CanvasGroup>().alpha = 0;
        }
		else {
			Debug.LogError("Ant does not have health text assigned.");
		}
    }

	public void FadeInHealthUI() {
		healthFadeScript.FadeInUI(0.5f);
	}

	public void FadeOutHealthUI() {
		if (healthTextBackground.activeInHierarchy) {
            healthFadeScript.FadeOutUI(0.5f);
        }
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
		Transform model = transform.GetChild(0).transform;
		if (movement.x < 0) {
			ResetAnimation();
			ChangeAnimation("Walking");
			model.rotation = Quaternion.Euler(0, 90, 0);
		} else if (movement.x > 0) {
			ResetAnimation();
			ChangeAnimation("Walking");
			model.rotation = Quaternion.Euler(0, -90, 0);
		} else {
			ResetAnimation();
		}
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
		} 
		else {
			health -= damage;
		}

		healthChangeText.color = Color.red;
		healthChangeText.text = "- " + damage;
		healthChangeText.gameObject.SetActive(true);
		healthChangeText.GetComponent<FadeScript>().FadeOutUI(2.0f);

		healthText.text = health.ToString();

		if (health <= 0) {
			OnDeath();
		}
	}

	public void OnDeath() {
        if (antInfo.IsQueen == true) {
            foreach (GameObject ant in player.AntList) {
                ant.GetComponent<Ant>().TakeDamage(turnManager.DamageToDealOnQueenDeath);
            }
        }

        if (isDrowning == false && isCrushed == false) {
			ResetAnimation();
			ChangeAnimation("Flailing");
			FindFirstObjectByType<CameraSystem>().AddNewCameraTarget(transform);
            CameraSystem.onIterationFinished += DestroyAnt;
        }
		else {
			ResetAnimation();
			ChangeAnimation("Dying");
			Destroy(antInfo.IsQueen, player);
		}
    }

	private void DestroyAnt(Transform _transform) {
		if (_transform == transform) {
			Destroy(antInfo.IsQueen, player);
		}
	}

	private void Destroy(bool isQueen, Player player) {
		if (isQueen) {
			player.RemoveQueen();
		}
		else {
			player.RemoveAnt(gameObject);
		}

		CheckGameOver();

        TurnManager.onTurnEnded -= FadeOutHealthUI;
        WeaponManager.onOpenWeaponsMenu -= FadeInHealthUI;
        WeaponManager.onCloseWeaponsMenu -= FadeOutHealthUI;
        CameraSystem.onIterationFinished -= DestroyAnt;

        Destroy(gameObject);
	}

	private void CheckGameOver() {
        Dictionary<int, int> antsRemaining = new Dictionary<int, int>();

        for (int i = 0; i < turnManager.PlayerList.Count; i++) {
            antsRemaining.Add(turnManager.PlayerList[i].playerInfo.playerNum, turnManager.PlayerList[i].AntList.Count);

            if (turnManager.PlayerList[i].QueenAnt != null) {
                antsRemaining[turnManager.PlayerList[i].playerInfo.playerNum] += 1;
            }
        }

        if (antsRemaining.TryGetValue(player.playerInfo.playerNum, out int amount)) {
            if (turnManager.PlayerList.Count == 1 && amount - 1 == 0) {
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

	public void HealDamage(int healthToHeal) {
		health += healthToHeal;

        healthChangeText.color = Color.green;
        healthChangeText.text = "+ " + healthToHeal;
        healthChangeText.gameObject.SetActive(true);
        healthChangeText.GetComponent<FadeScript>().FadeOutUI(1.0f);

        healthText.text = health.ToString();
    }

	public int GetHealth() {
		return health;
	}

	public void SetHealth(int healthToSet) {
		health = healthToSet;
	}

	public void OnJump() {
		if (canJump && canMove) {
			ResetAnimation();
			ChangeAnimation("Jumping");
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
		if (canMove) {
			if (hasStatDrop) {
				if (statDrops.Contains(EffectSO.StatDropType.Speed)) {
					float percentage = 0;
					for (int i = 0; i < effects.Count; i++) {
						if (effects[i].effectInfo.statDropType == EffectSO.StatDropType.Speed) {
							percentage = effects[i].effectInfo.percentToDropBy;
							break;
						}
					}
					transform.Translate(antInfo.moveSpeed * Time.deltaTime * moveVector * percentage, Space.World);

				} else {
					
					transform.Translate(antInfo.moveSpeed * Time.deltaTime * moveVector, Space.World);
				}

			} else {
				transform.Translate(antInfo.moveSpeed * Time.deltaTime * moveVector, Space.World);
			}
		}

		transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
    }

    private void OnCollisionEnter(Collision collision) {
		if(Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit ray, 3.0f)) {
			ResetAnimation();
			//ChangeAnimation("Landing");
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

	public void ChangeAnimation(string nextAnimState) {
		animator.SetBool(nextAnimState, true);
	}

	public void ResetAnimation() {
		animator.SetBool("Walking", false);
		animator.SetBool("UsingBackWeapon", false);
		animator.SetBool("Dying", false);
		animator.SetBool("Standing", false);
		animator.SetBool("Jumping", false);
		animator.SetBool("Landing", false);
		animator.SetBool("Flailing", false);
	}
}
