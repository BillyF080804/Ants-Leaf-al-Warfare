using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeAttack : QueenAttack {
	[SerializeField]
	GameObject beeSummon;

	public override void ActivateAttack(int attackLevel, Ant antInfoScript, Vector3 position) {
		Debug.Log("Updated");
		Debug.Log(attackLevel);
		for (int i = 0; i < attackLevel; i++) {
			BeeScript beeScript = Instantiate(beeSummon, position, Quaternion.identity).GetComponent<BeeScript>();
			beeScript.InitialiseValues(antInfoScript, attackLevel);
			
		}

	}
}
