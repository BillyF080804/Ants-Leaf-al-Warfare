using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PharaohAttack : QueenAttack {
	[SerializeField]
	GameObject mummySummon;

	[SerializeField]
	EffectScript mummyEffect;

	[SerializeField]
	int baseHealth = 50;

	[SerializeField]
	int healthPerLevel = 15;



	public override void ActivateAttack(int attackLevel, Ant antInfoScript, Vector3 position, TurnManager turnManager) {
		if (attackLevel != 3 && attackLevel > 0) {
			for (int i = 0; i < 2; i++) {
				GameObject tempMummy = Instantiate(mummySummon, position + new Vector3(Random.Range(-5,5),0,0), Quaternion.identity);
				tempMummy.GetComponent<MummyScript>().SetHealth(baseHealth + healthPerLevel * attackLevel);
				tempMummy.GetComponent<MummyScript>().InitialiseMummy(mummyEffect,antInfoScript.ownedPlayer);
				turnManager.PlayerList[(int)antInfoScript.ownedPlayer].AddNewAnt(tempMummy);
			}
		} else {
			for (int i = 0; i < 3; i++) {
				GameObject tempMummy = Instantiate(mummySummon, position + new Vector3(Random.Range(-5, 5), 0, 0), Quaternion.identity);
				tempMummy.GetComponent<MummyScript>().SetHealth(baseHealth + healthPerLevel * 2);
				tempMummy.GetComponent<MummyScript>().InitialiseMummy(mummyEffect, antInfoScript.ownedPlayer);
				turnManager.PlayerList[(int)antInfoScript.ownedPlayer].AddNewAnt(tempMummy);
			}
		}
	}

	public override void InitialiseValues(GameObject attackInfo) {
		mummySummon = attackInfo.GetComponent<PharaohAttack>().mummySummon;
		mummyEffect = attackInfo.GetComponent<PharaohAttack>().mummyEffect;
		baseHealth = attackInfo.GetComponent<PharaohAttack>().baseHealth;
		healthPerLevel = attackInfo.GetComponent<PharaohAttack>().healthPerLevel;
	}
}
