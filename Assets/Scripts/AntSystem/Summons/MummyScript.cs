using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyScript : Ant
{
	[SerializeField]
	int turnsAlive;

	EffectScript effect;

	public void InitialiseMummy(EffectScript _effect) {
		effect = _effect;
	}


	public void DecreaseTurnsAlive() {
		turnsAlive--;
		if (turnsAlive <= 0) {
			OnDeath();
		}
	}
}
