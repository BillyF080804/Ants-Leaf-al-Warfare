using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript : MonoBehaviour {
	[SerializeField]
	EffectSO effectInfo;

	int turnsLeft;

	private void Start() {
		turnsLeft = effectInfo.amountOfTurns;
	}

	public void ApplyEffect(Ant ant) {
		switch (effectInfo.effectType) {
			case EffectSO.EffectType.DamageOverTime: {
				if (turnsLeft > 0) {
					ant.TakeDamage(effectInfo.amountOfDamagePerTurn);
					turnsLeft--;
				} else if (effectInfo.multiTurn == false) {
					ant.TakeDamage(effectInfo.amountOfDamagePerTurn);
					RemoveEffect(ant);
				} else {
					RemoveEffect(ant);
				}

				break;
			}
			case EffectSO.EffectType.StatDrop: {
				break;
			}
			case EffectSO.EffectType.LifeSteal: { 
				break; 
			}
		}
	}

	public void AddEffect(Ant ant) { 
		ant.effects.Add(this);
	}

	public void RemoveEffect(Ant ant) {
		ant.effects.Remove(this);
		Destroy(this);
	}
}
