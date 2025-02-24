using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class QueenAntScript : Ant {

	[Header("Game Info")]
	public int attackLevel = 1;
	bool usedAttack = false;	

	public void CheckAttackTurn() {
		if (!usedAttack) {
			if (attackLevel < 4) {
				attackLevel++;
			}
			
		}
		usedAttack = false;
	}

	public void SpecialAttack() {
		if(attackLevel != 0) {
			if (antInfo.queenType == AntSO.QueenType.Bee) {
				antInfo.queenAttack.ActivateAttack(attackLevel, this, transform.position);
			} else {
				antInfo.queenAttack.ActivateAttack(attackLevel, this);
			}

			attackLevel = 0;
			usedAttack = true;
		}

	}
}
