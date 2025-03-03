using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect System/New Effect", fileName = "New Effect")]
public class EffectSO : ScriptableObject {
	public enum EffectType {
		NULL = -1,
		DamageOverTime,
		StatDrop,
		LifeSteal
	}

	public enum StatDropType {
		NULL = -1,
		Speed,
		Defense
	}

	[Header("Effect Info")]
	public string effectName;

	public EffectType effectType;

	public bool multiTurn;

	//damage effect settings
	[HideInInspector] public int amountOfTurns;
	[HideInInspector] public int amountOfDamagePerTurn;

	//stat drop settings
	//[HideInInspector] 
	public StatDropType statDropType;

	public float percentToDropBy;

}

#if (UNITY_EDITOR)
[CustomEditor(typeof(EffectSO))]
public class EffectSOEditor : Editor {
	[InitializeOnEnterPlayMode]
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		EffectSO baseEffect = (EffectSO)target;

		if (baseEffect.multiTurn) {
			EditorGUILayout.Space(15);
			EditorGUILayout.LabelField("Damage Over Time Settings", EditorStyles.boldLabel);
			baseEffect.amountOfTurns = EditorGUILayout.IntField("Amount of turns", baseEffect.amountOfTurns);
			baseEffect.amountOfDamagePerTurn = EditorGUILayout.IntField("Damage per turn", baseEffect.amountOfDamagePerTurn);
		}

		if(baseEffect.effectType == EffectSO.EffectType.StatDrop) {
			//baseEffect.statDropType = EditorGUILayout.ObjectField("Type of Stat Drop", baseEffect.statDropType, typeof(EffectSO.StatDropType), false);
		}
	}
}
#endif

