using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript : MonoBehaviour {
	[SerializeField]
	EffectSO effectInfo;

	int turnsLeft;
	int effectLevel;
	float lifeStealPercent;

	private void Start() {
		turnsLeft = effectInfo.amountOfTurns;
	}

	public void ApplyEffect(Ant ant) {
		switch (effectInfo.effectType) {
			case EffectSO.EffectType.DamageOverTime: {
				if (effectInfo.multiTurn == false) {
					ant.TakeDamage(effectInfo.amountOfDamagePerTurn);
					RemoveEffect(ant);
				} else if(turnsLeft > 0) {
					ant.TakeDamage(effectInfo.amountOfDamagePerTurn);
					turnsLeft--;
				} else {
					RemoveEffect(ant);
				}

				break;
			}
			case EffectSO.EffectType.StatDrop: {
				break;
			}
			case EffectSO.EffectType.LifeSteal: {
				if (ant.hasLifeSteal) {
					ant.hasLifeSteal = !ant.hasLifeSteal;
					ant.lifeStealPercent = 0;
					RemoveEffect(ant);
				}
				ant.hasLifeSteal = true;
				ant.lifeStealPercent = lifeStealPercent;
				break; 
			}
		}
	}

	public void AddEffect(Ant ant) { 
		ant.effects.Add(this);
	}

	public void AddEffect(Ant ant, float lifeStealLevel) {
		ant.effects.Add(this);
		lifeStealPercent = lifeStealLevel;
		ApplyEffect(ant);
	}

	public void RemoveEffect(Ant ant) {
		ant.effects.Remove(this);
	}
}
