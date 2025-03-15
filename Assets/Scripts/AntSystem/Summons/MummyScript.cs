using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MummyScript : Ant
{
	[SerializeField]
	int turnsAlive;

	EffectScript effect;
	public int attackDamage;


	public void InitialiseMummy(EffectScript _effect, PlayerList _ownedPlayer) {
		effect = _effect;
		ownedPlayer = _ownedPlayer;
	}


	public void DecreaseTurnsAlive() {
		turnsAlive--;
		if (turnsAlive <= 0) {
			OnDeath();
		}
	}

	public void Attack() {
		List<Collider> ants = Physics.OverlapSphere(transform.position, 3.0f).Where(x => x.GetComponent<Ant>()).ToList();
		if(ants.Count > 0) {
			Debug.Log("test");
			if(ants.Count > 1) {
				float distance = Mathf.Infinity;
				Ant closestAnt = null;
				for(int i = 0; i < ants.Count; i++) {
					if (ants[i].GetComponent<Ant>().ownedPlayer != ownedPlayer) {
						float tempDist = (ants[i].transform.position - transform.position).magnitude;
						if (tempDist < distance) {
							distance = tempDist;
							closestAnt = ants[i].GetComponent<Ant>();
						}
					}

				}
				if(closestAnt != null) {
					closestAnt.TakeDamage(attackDamage);
					effect.AddEffect(closestAnt);
				}


			} else {
				if(ants[0].GetComponent<Ant>().ownedPlayer != ownedPlayer) {
					ants[0].GetComponent<Ant>().TakeDamage(attackDamage);
					effect.AddEffect(ants[0].GetComponent<Ant>());
				}
				
			}
			turnManager.EndTurn();
		}

	}
}
