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
				GameObject tempMummy = Instantiate(mummySummon);
				tempMummy.GetComponent<MummyScript>().SetHealth(baseHealth + healthPerLevel * attackLevel);
				tempMummy.GetComponent<MummyScript>().InitialiseMummy(mummyEffect);
				turnManager.PlayerList[(int)antInfoScript.ownedPlayer].AddNewAnt(tempMummy);
			}
		} else {
			for (int i = 0; i < 3; i++) {
				GameObject tempMummy = Instantiate(mummySummon);
				tempMummy.GetComponent<MummyScript>().SetHealth(baseHealth + healthPerLevel * 2);
				turnManager.PlayerList[(int)antInfoScript.ownedPlayer].AddNewAnt(tempMummy);
			}
		}
	}
}
