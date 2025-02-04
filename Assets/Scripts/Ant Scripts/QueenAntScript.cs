using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using static AntScript;

public class QueenAntScript : Ant {
	[Header("Queen Info")]
	public QueenAntSO queenAntInfo;

	[Header("Game Info")]
	public int attackLevel = 0;

	public void PassTurn() {
		attackLevel++;
	}

	public void Attack() {
		switch (queenAntInfo.queenType) {
			case QueenAntSO.QueenType.Fire: {
				FireAttack();
				break;
			}
			case QueenAntSO.QueenType.Bee: {
				BeeAttack();
				break;
			}
			case QueenAntSO.QueenType.Pharaoh: {
				PharaohAttack();
				break;
			}
			case QueenAntSO.QueenType.Weaver: {
				WeaverAttack();
				break;
			}
			case QueenAntSO.QueenType.Dracula: {
				DraculaAttack();
				break;
			}
			case QueenAntSO.QueenType.Bullet: {
				BulletAttack();
				break;
			}
		}
		attackLevel = 0;
	}

	void FireAttack() {
		int damageToDeal = queenAntInfo.damage * attackLevel;
	}
	void BeeAttack() {
		int damageToDeal = queenAntInfo.damage * attackLevel;
	}
	void PharaohAttack() {
		int damageToDeal = queenAntInfo.damage * attackLevel;
	}
	void WeaverAttack() {
		int damageToDeal = queenAntInfo.damage * attackLevel;
	}
	void DraculaAttack() {
		int damageToDeal = queenAntInfo.damage * attackLevel;
	}
	void BulletAttack() {
		int damageToDeal = queenAntInfo.damage * attackLevel;
	}

	public int GetHealth() {
		return queenAntInfo.health;
	}

	public void OnMove(InputValue value) {
		Vector2 movement = value.Get<Vector2>();
		moveVector = new Vector3(movement.x, 0, 0);
	}

	public void OnJump() {
		if (canJump) {
			Vector2 Force = new Vector2(0, queenAntInfo.jumpHeight);
			GetComponent<Rigidbody>().AddForce(Force, ForceMode.Impulse);
			canJump = false;
		}
	}

    public void TakeDamage(int Damage) {
        queenAntInfo.health -= Damage;
    }



	private void Update() {
		transform.Translate(queenAntInfo.moveSpeed * Time.deltaTime * moveVector);
	}
}
