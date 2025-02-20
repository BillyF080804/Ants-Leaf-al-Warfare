using System.Collections;
using System.Collections.Generic;
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

	[SerializeField]
	private int health;
	private Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
		health = antInfo.health;
    }


    public void OnMove(InputValue value) {
		Vector2 movement = value.Get<Vector2>();
		moveVector = new Vector3(movement.x, 0, 0);
	}

	public void TakeDamage(int damage) {
		health -= damage;

		Debug.Log("Ant Health: " + health);
	}

	public void HealDamage(int damageToHeal) {
		health += damageToHeal;
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
}
