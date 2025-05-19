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
    [HideInInspector] public StatDropType statDropType;
    [HideInInspector] public float percentToDropBy;

    [SerializeField] public ParticleSystem particleEffect;
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

        if (baseEffect.effectType == EffectSO.EffectType.StatDrop) {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Stat Drop Settings", EditorStyles.boldLabel);
            baseEffect.statDropType = (EffectSO.StatDropType)EditorGUILayout.EnumPopup("Stat Drop Type", baseEffect.statDropType);
            baseEffect.percentToDropBy = EditorGUILayout.FloatField("Percent To Drop By", baseEffect.percentToDropBy);

        }
    }
}
#endif

