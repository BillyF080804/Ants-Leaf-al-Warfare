using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeAttack : QueenAttack {
	[SerializeField]
	GameObject beeSummon;
	public override void ActivateAttack(int attackLevel, Ant antInfoScript) {
		BeeScript beeScript = Instantiate(beeSummon).GetComponent<BeeScript>();
		beeScript.InitialiseValues(antInfoScript);
		Debug.Log("Updated");
	}
}
