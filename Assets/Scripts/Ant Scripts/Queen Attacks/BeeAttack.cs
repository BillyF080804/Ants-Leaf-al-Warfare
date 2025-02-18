using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeAttack : QueenAttack {
	[SerializeField]
	GameObject beeSummon;

	public override void ActivateAttack(int attackLevel, Ant antInfoScript) {
		for(int i = 0; i < attackLevel; i++) {
			BeeScript beeScript = Instantiate(beeSummon).GetComponent<BeeScript>();
			beeScript.InitialiseValues(antInfoScript, attackLevel);
			Debug.Log("Updated");
		}

	}
}
