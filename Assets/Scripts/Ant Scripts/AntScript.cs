using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AntScript : MonoBehaviour {
	public enum PlayerList {
		Player1 = 0,
		Player2 = 1,
		Player3 = 2,
		Player4 = 3,
	}

	[Header("Ant Info")]
	public BaseAntSO antInfo;

	[Header("Game Info")]
	public PlayerList ownedPlayer;

	public Vector3 moveVector = Vector3.zero;
	bool canJump = true;

	public bool hasHadTurn = false;
	private TurnManager turnManager;

	private void Start() {
		turnManager = FindFirstObjectByType<TurnManager>();
	}

	public void OnMove(InputValue value) {
        Vector2 movement = value.Get<Vector2>();
        moveVector = new Vector3(movement.x, 0, 0);
    }

	public void OnJump() {
        if (canJump) {
            Vector2 Force = new Vector2(0, antInfo.jumpHeight);
            GetComponent<Rigidbody>().AddForce(Force, ForceMode.Impulse);
            canJump = false;
        }
    }

	public void TakeDamage(int Damage) {
		antInfo.health -= Damage;
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
