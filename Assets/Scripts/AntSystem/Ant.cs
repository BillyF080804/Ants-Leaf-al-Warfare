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

	private int health;
	private Rigidbody rb;
	private Slider healthSlider;
	private TurnManager turnManager;

    private void Awake() {
		turnManager = FindFirstObjectByType<TurnManager>();
        rb = GetComponent<Rigidbody>();
		healthSlider = GetComponentInChildren<Slider>();
		health = antInfo.health;

		healthSlider.maxValue = health;
		healthSlider.value = health;
    }


    public void OnMove(InputValue value) {
		Vector2 movement = value.Get<Vector2>();
		moveVector = new Vector3(movement.x, 0, 0);
	}

	public void TakeDamage(int damage) {
		health -= damage;
		healthSlider.value = health;

		if (health <= 0) {
			OnDeath();
		}
	}

	private void OnDeath() {
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

	public void OnJump() {
		if (canJump) {
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
        transform.Translate(antInfo.moveSpeed * Time.deltaTime * moveVector);
    }

    private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.transform.position.y < gameObject.transform.position.y) {
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
