using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BeeScript : MonoBehaviour
{
	public Ant.PlayerList ownedPlayer;
	public Ant antToAttack;
	[SerializeField]
	int damageToDeal;
	[SerializeField]
	int speed = 5;

	[SerializeField]
	List<EffectScript> effects;

	public void InitialiseValues(Ant antInfoScript, int attackLevel) {
		ownedPlayer = antInfoScript.ownedPlayer;
		damageToDeal = damageToDeal * attackLevel;
		Attack();
	}

	public void Attack() {
		antToAttack = ClosestAnt();
		Vector3.MoveTowards(transform.position, antToAttack.transform.position, 1);
	}

	Ant ClosestAnt() {
		Ant[] antList = GameObject.FindObjectsOfType<Ant>();
		float currentDistance = Mathf.Infinity;
		Ant closestAnt = null;
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

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.GetComponent<Ant>() == antToAttack) {
			antToAttack.TakeDamage(damageToDeal);
			for(int i = 0; i<effects.Count; i++) {
				effects[i].AddEffect(antToAttack);
				if (effects[i].effectInfo.effectType == EffectSO.EffectType.StatDrop) {
					effects[i].ApplyEffect(antToAttack);
				}
			}
			Destroy(gameObject);
		}
	}

	private void Update() {
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, antToAttack.transform.position, step);
		
	}
}

