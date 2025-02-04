using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ant : MonoBehaviour
{
	public enum PlayerList {
		Player1 = 0,
		Player2 = 1,
		Player3 = 2,
		Player4 = 3,
	}


	[Header("Ant Info")]
	public AntSO antInfo;


	public PlayerList ownedPlayer;
	public Vector3 moveVector = Vector3.zero;
	public bool canJump = true;


	public void OnMove(InputValue value) {
		Vector2 movement = value.Get<Vector2>();
		moveVector = new Vector3(movement.x, 0, 0);
	}

	public void TakeDamage(int Damage) {
		antInfo.health -= Damage;
	}

	public int GetHealth() {
		return antInfo.health;
	}

	public void OnJump() {
		if (canJump) {
			Vector2 Force = new Vector2(0, antInfo.jumpHeight);
			GetComponent<Rigidbody>().AddForce(Force, ForceMode.Impulse);
			canJump = false;
		}
	}

	private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.transform.position.y < gameObject.transform.position.y) {
			canJump = true;
		}
	}
}
