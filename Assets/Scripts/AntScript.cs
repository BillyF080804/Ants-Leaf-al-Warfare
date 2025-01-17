using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AntScript : MonoBehaviour
{
	public BaseAntSO antInfo;
	Vector3 moveVector = Vector3.zero;
	bool CanJump = true;

	private void OnMove(InputValue value) {
		Vector2 movement = value.Get<Vector2>();
		moveVector = new Vector3(movement.x, 0, 0);
	}

	private void OnJump() {
		if (CanJump) {
			Vector2 Force = new Vector2(0, antInfo.jumpHeight);
			GetComponent<Rigidbody>().AddForce(Force, ForceMode.Impulse);
			CanJump = false;
		}
	}

	void TakeDamage(int Damage) {
		antInfo.health -= Damage;
	}

	private void Update() {
		transform.Translate(antInfo.moveSpeed * Time.deltaTime * moveVector);
	}

	private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.transform.position.y < gameObject.transform.position.y) {
			CanJump = true;
		}
	}
}
