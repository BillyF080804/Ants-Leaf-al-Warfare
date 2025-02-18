using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class QueenAntScript : Ant {

	[Header("Game Info")]
	public int attackLevel = 1;

	public void PassTurn() {
		attackLevel++;
	}

	public void SpecialAttack() {
		Debug.Log("HI");
		antInfo.queenAttack.ActivateAttack(attackLevel, this);
		attackLevel = 0;
	}
}
