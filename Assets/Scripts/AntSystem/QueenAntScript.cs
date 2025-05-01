using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QueenBaseAntScript : Ant {

	[Header("Queen Info")]
	[SerializeField] private LayerMask queenLayerMask;
	private int attackLevel = 1;
	private bool usedAttack = false;

    [Header("Queen Ant Settings")]
    [SerializeField] private SkinnedMeshRenderer hat;
    [SerializeField] private SkinnedMeshRenderer leafWrap;
	[SerializeField] private QueenAttack AttackScript;

    private void Start() {
		InitialiseQueenAttack();

	}

	void InitialiseQueenAttack() {
		QueenAttack tempAttack = Instantiate(antInfo.queenAttack);
		if (tempAttack.gameObject.GetComponent<BeeAttack>() != null) {
			AttackScript = this.AddComponent<BeeAttack>();
		} else if (tempAttack.gameObject.GetComponent<BulletAttack>() != null) {
			AttackScript = this.AddComponent<BulletAttack>();
		} else if (tempAttack.gameObject.GetComponent<DraculaAttack>() != null) {
			AttackScript = this.AddComponent<DraculaAttack>();
		} else if (tempAttack.gameObject.GetComponent<IceAttack>() != null) {
			AttackScript = this.AddComponent<IceAttack>();
		} else if (tempAttack.gameObject.GetComponent<PharaohAttack>() != null) {
			AttackScript = this.AddComponent<PharaohAttack>();
		} else if (tempAttack.gameObject.GetComponent<WeaverAttack>() != null) {
			AttackScript = this.AddComponent<WeaverAttack>();
		}
		
		AttackScript.InitialiseValues(tempAttack.gameObject);
		Destroy(tempAttack.gameObject);
	}

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
				AttackScript.ActivateAttack(attackLevel, this);
			} else if (antInfo.queenType == AntSO.QueenType.Pharaoh) {
				AttackScript.ActivateAttack(attackLevel, this, transform.position, turnManager);
			} else {
				AttackScript.ActivateAttack(attackLevel, this, transform.position);
			}

            attackLevel = 0;
			usedAttack = true;
            turnManager.HideMainUI();
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