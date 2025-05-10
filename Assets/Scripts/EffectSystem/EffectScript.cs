using System.Collections;
using UnityEngine;

public class EffectScript : MonoBehaviour {
	[SerializeField]
	public EffectSO effectInfo;

	int turnsLeft;
	int effectLevel;
	float lifeStealPercent;

	GameObject tempParticle;

	private void Start() {

		turnsLeft = effectInfo.amountOfTurns;
	}

	public void ApplyEffect(Ant ant) {
		switch (effectInfo.effectType) {
			case EffectSO.EffectType.DamageOverTime: {
				DamageOverTimeEffect(ant);
				break;
			}
			case EffectSO.EffectType.StatDrop: {
				StatDropEffect(ant);
				break;
			}
			case EffectSO.EffectType.LifeSteal: {
				LifestealEffect(ant);
				break;
			}
		}
		DelayParticle(ant);
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


	void DamageOverTimeEffect(Ant ant) {
		if (effectInfo.multiTurn == false) {
			ant.TakeDamage(effectInfo.amountOfDamagePerTurn);
			RemoveEffect(ant);
		} else if (turnsLeft > 0) {
			ant.TakeDamage(effectInfo.amountOfDamagePerTurn);
			turnsLeft--;
		} else {
			RemoveEffect(ant);
		}
	}

	void StatDropEffect(Ant ant) {
		switch (effectInfo.statDropType) {
			case EffectSO.StatDropType.Speed: {
				ant.hasStatDrop = true;
				ant.statDrops.Add(EffectSO.StatDropType.Speed);
				break;
			}
			case EffectSO.StatDropType.Defense: {
				ant.hasStatDrop = true;
				ant.statDrops.Add(EffectSO.StatDropType.Defense);
				break;
			}
		}

	}

	void LifestealEffect(Ant ant) {
		if (ant.hasLifeSteal) {
			ant.hasLifeSteal = !ant.hasLifeSteal;
			ant.lifeStealPercent = 0;
			RemoveEffect(ant);
		}
		ant.hasLifeSteal = true;
		ant.lifeStealPercent = lifeStealPercent;
	}


	public void DelayParticle(Ant ant) {
		if (effectInfo.particleEffect != null) {
            tempParticle = Instantiate(effectInfo.particleEffect.gameObject, ant.gameObject.transform.position, Quaternion.identity);
        }
		else {
			Debug.LogError("Please Assign A Particle System for the effect system.");
		}

		ant.StartCoroutine(ant.WaitForEffect(3, this));
	}

	public void DestroyEffect() {
		Destroy(tempParticle);
	}
}
