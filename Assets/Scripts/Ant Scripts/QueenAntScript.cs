using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class QueenAntScript : Ant {

	[Header("Game Info")]
	public int attackLevel = 0;
	[SerializeField]
	GameObject beeSummon;
	[SerializeField]
	GameObject pharaohSummon;

	public void PassTurn() {
		attackLevel++;
	}

	public void SpecialAttack() {
		switch (antInfo.queenType) {
			case AntSO.QueenType.Fire: {
				FireAttack();
				break;
			}
			case AntSO.QueenType.Bee: {
				BeeAttack();
				break;
			}
			case AntSO.QueenType.Pharaoh: {
				PharaohAttack();
				break;
			}
			case AntSO.QueenType.Weaver: {
				WeaverAttack();
				break;
			}
			case AntSO.QueenType.Dracula: {
				DraculaAttack();
				break;
			}
			case AntSO.QueenType.Bullet: {
				BulletAttack();
				break;
			}
		}
		attackLevel = 0;
	}

	void FireAttack() {
		int damageToDeal = antInfo.damage * attackLevel;
	}
	void BulletAttack() {
		int damageToDeal = antInfo.damage * attackLevel;
	}

	void BeeAttack() {
		int damageToDeal = antInfo.damage * attackLevel;
	}
	void PharaohAttack() {
		int damageToDeal = antInfo.damage * attackLevel;
	}

	void WeaverAttack() {
		int damageToDeal = antInfo.damage * attackLevel;
	}
	void DraculaAttack() {
		int damageToDeal = antInfo.damage * attackLevel;
	}
}
