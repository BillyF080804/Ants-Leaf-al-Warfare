using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenAntScript : Ant {

	[Header("Queen Info")]
	[SerializeField] private LayerMask queenLayerMask;
	public int attackLevel = 1;
	bool usedAttack = false;

	[Header("Cosmetics")]
	[SerializeField] private MeshRenderer antRenderer;

	public void CheckAttackTurn() {
		if (!usedAttack) {
			if (attackLevel < 3) {
				attackLevel++;
			}			
		}

		usedAttack = false;
	}

	public void SpecialAttack() {
		if(attackLevel != 0) {
			if (antInfo.queenType == AntSO.QueenType.Bee || antInfo.queenType == AntSO.QueenType.Weaver) {
				antInfo.queenAttack.ActivateAttack(attackLevel, this, transform.position);
			} else {
				antInfo.queenAttack.ActivateAttack(attackLevel, this);
			}

			attackLevel = 0;
			usedAttack = true;
		}
	}

	public void SetQueenInvalidPos() {
		antRenderer.material.color = new Color(1, 0, 0);
	}

	public void SetQueenValidPos() {
		antRenderer.material.color = new Color(0, 1, 0);
	}

	public void SetQueenToTeamColour(Color teamColor) {
		antRenderer.material.color = teamColor;
	}

	public LayerMask GetQueenLayerMask() {
		return queenLayerMask;
	}
}