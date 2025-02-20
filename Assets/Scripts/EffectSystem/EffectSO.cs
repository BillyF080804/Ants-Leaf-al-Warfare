using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect System/New Effect", fileName = "New Effect")]
public class EffectSO : ScriptableObject {
	public string effectName;
	public int amountOfTurns;

	enum EffectType {
		Damage,

	}
}
