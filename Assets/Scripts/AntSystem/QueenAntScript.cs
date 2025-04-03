using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenBaseAntScript : Ant {

	[Header("Queen Info")]
	[SerializeField] private LayerMask queenLayerMask;
	public int attackLevel = 1;
	bool usedAttack = false;

    [Header("Queen Ant Settings")]
    [SerializeField] private SkinnedMeshRenderer hat;
    [SerializeField] private SkinnedMeshRenderer leafWrap;

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
			if(antInfo.queenType == AntSO.QueenType.Dracula) {
				antInfo.queenAttack.ActivateAttack(attackLevel, this);
			} else if (antInfo.queenType == AntSO.QueenType.Pharaoh) {
				antInfo.queenAttack.ActivateAttack(attackLevel, this, transform.position, turnManager);
			} else {
				antInfo.queenAttack.ActivateAttack(attackLevel, this, transform.position);
			}

			attackLevel = 0;
			usedAttack = true;
			turnManager.EndTurn();
		}
	}

    public void ChangeAntColors(Color newColor) {
        hat.material.SetColor("_MainColours", newColor);
        leafWrap.material.SetColor("_MainColours", newColor);
    }

    public LayerMask GetQueenLayerMask() {
		return queenLayerMask;
	}
}