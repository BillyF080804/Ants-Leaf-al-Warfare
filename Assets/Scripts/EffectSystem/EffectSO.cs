using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect System/New Effect", fileName = "New Effect")]
public class EffectSO : ScriptableObject {
	public string effectName;

	public bool multiTurn;

	public int amountOfTurns;

	public enum EffectType {
		NULL = -1,
		DamageOverTime,
		StatDrop,
		LifeSteal
	}

	public EffectType effectType;	

	public int amountOfDamagePerTurn;



}
