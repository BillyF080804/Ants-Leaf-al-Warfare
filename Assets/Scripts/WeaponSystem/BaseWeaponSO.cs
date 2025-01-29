using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon System/New Weapon", fileName = "New Weapon")]
public class BaseWeaponSO : ScriptableObject {
    [Header("Weapon Info")]
    public string weaponName;
    public string weaponDescription;
    
    [Header("Weapon Cosmetics")]
    public Sprite weaponIcon = null;
    public GameObject weaponPrefab = null;

    [Header("Weapon Stats")]
    public float baseDamage = 20.0f;
    public float weaponSpeed = 25.0f;
    public bool useGravity = true;
    public bool explosive = false;

    [HideInInspector] public bool explodeOnImpact = true;
    [HideInInspector] public float explosionRange = 5.0f;
    [HideInInspector] public float explosionPower = 5.0f;
    [HideInInspector] public float fuseTimer = 5.0f;
}

#if (UNITY_EDITOR)
[CustomEditor(typeof(BaseWeaponSO))]
public class BaseWeaponSOEditor : Editor {
    [InitializeOnEnterPlayMode]
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        BaseWeaponSO baseWeapon = (BaseWeaponSO)target;

        if (baseWeapon.explosive) {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Explosion Settings", EditorStyles.boldLabel);
            baseWeapon.explosionRange = EditorGUILayout.FloatField("Explosion Range", baseWeapon.explosionRange);
            baseWeapon.explosionPower = EditorGUILayout.FloatField("Explosion Power", baseWeapon.explosionPower);
            baseWeapon.explodeOnImpact = EditorGUILayout.Toggle("Explode On Impact", baseWeapon.explodeOnImpact);

            if (!baseWeapon.explodeOnImpact) {
                baseWeapon.fuseTimer = EditorGUILayout.FloatField("Fuse Timer", baseWeapon.fuseTimer);
            }
        }
    }
}
#endif