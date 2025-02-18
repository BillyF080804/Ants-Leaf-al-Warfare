using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BeeScript : MonoBehaviour
{
	public Ant.PlayerList ownedPlayer;
	public Ant antToAttack;
	int damageToDeal;

	public void InitialiseValues(Ant antInfoScript, int attackLevel) {
		ownedPlayer = antInfoScript.ownedPlayer;
		damageToDeal = antInfoScript.antInfo.damage * attackLevel;
	}

	public void Attack() {
		antToAttack = ClosestAnt();
		Vector3.MoveTowards(transform.position, antToAttack.transform.position, 1);
	}

	Ant ClosestAnt() {
		Ant[] antList = GameObject.FindObjectsOfType<Ant>();
		float currentDistance = Mathf.Infinity;
		Ant closestAnt = new Ant();
		foreach (Ant ant in antList) {
			float tempDist = Distance(ant);
            if (tempDist < currentDistance)
            {
                if(ant.ownedPlayer != ownedPlayer) {
					currentDistance = tempDist;
					closestAnt = ant;
				}
            }
        }
		return closestAnt;
	}

	float Distance(Ant ant) {
		return Vector3.Magnitude(ant.transform.position - transform.position);
	}

	private void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.GetComponent<Ant>() == antToAttack) {
			antToAttack.TakeDamage(damageToDeal);
		}
	}
}
