using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraculaAttack : QueenAttack {

	public EffectScript effect;

	[Tooltip("Use decimal versions of the percentage you want")]
	public List<float> percentages;

	public override void ActivateAttack(int attackLevel, Ant antInfoScript) {
		Ant[] antList = FindObjectsOfType<Ant>();
		List<Ant> antList2 = new List<Ant>();
		for (int i = 0; i < antList.Length; i++) {
			if (antList[i].ownedPlayer == antInfoScript.ownedPlayer) {
				antList2.Add(antList[i]);

			}

		}

		int index = attackLevel - 1;
		for (int i = 0; i < antList2.Count; i++) {
			
			if(attackLevel < percentages.Count) {
				effect.AddEffect(antList2[i], percentages[index]);
			}
			
		}
	}

	public override void InitialiseValues(GameObject attackInfo) {
		effect = attackInfo.GetComponent<DraculaAttack>().effect;
		percentages = attackInfo.GetComponent<DraculaAttack>().percentages;

	}
}
