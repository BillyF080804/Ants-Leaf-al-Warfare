using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using static AntScript;

public class QueenAntScript : MonoBehaviour {

	[Header("Queen Info")]
	public BaseQueenAntSO queenAntInfo;

	[Header("Game Info")]
	public PlayerList ownedPlayer;
	Vector3 moveVector = Vector3.zero;
	bool canJump = true;
	public int attackLevel = 0;

	public void PassTurn() {
		attackLevel++;
	}

	public void Attack() {
		switch (queenAntInfo.queenType) {
			case BaseQueenAntSO.QueenType.Fire: {
				FireAttack();
				break;
			}
			case BaseQueenAntSO.QueenType.Bee: {
				BeeAttack();
				break;
			}
			case BaseQueenAntSO.QueenType.Pharaoh: {
				PharaohAttack();
				break;
			}
			case BaseQueenAntSO.QueenType.Weaver: {
				WeaverAttack();
				break;
			}
			case BaseQueenAntSO.QueenType.Dracula: {
				DraculaAttack();
				break;
			}
			case BaseQueenAntSO.QueenType.Bullet: {
				BulletAttack();
				break;
			}
		}
		attackLevel = 0;
	}

	void FireAttack() {
		int damageToDeal = queenAntInfo.Damage * attackLevel;
	}
	void BeeAttack() {
		int damageToDeal = queenAntInfo.Damage * attackLevel;
	}
	void PharaohAttack() {
		int damageToDeal = queenAntInfo.Damage * attackLevel;
	}
	void WeaverAttack() {
		int damageToDeal = queenAntInfo.Damage * attackLevel;
	}
	void DraculaAttack() {
		int damageToDeal = queenAntInfo.Damage * attackLevel;
	}
	void BulletAttack() {
		int damageToDeal = queenAntInfo.Damage * attackLevel;
	}


	public int GetHealth() {
		return queenAntInfo.Health;
	}


	private void OnMove(InputValue value) {
		Vector2 movement = value.Get<Vector2>();
		moveVector = new Vector3(movement.x, 0, 0);
	}

	private void OnJump() {
		if (canJump) {
			Vector2 Force = new Vector2(0, queenAntInfo.jumpHeight);
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
