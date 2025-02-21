using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon System/New Weapon", fileName = "New Weapon")]
public class BaseWeaponSO : ScriptableObject {
    [Header("Weapon Info")]
    public string weaponName;
    public string weaponDescription;
    public bool unlimitedUses = true;
    
    [Header("Weapon Cosmetics")]
    public Sprite weaponIcon = null;
    public GameObject weaponPrefab = null;

    [Header("Weapon Settings")]
    public float cameraDelay = 4.0f;
    public int baseDamage = 20;
    public float weaponSpeed = 25.0f;
    public bool useGravity = true;
    [Tooltip("This will add some variance onto the speed & aim of the weapon when shot.")] public bool weaponRandomisation = false;
    public bool explosive = false;
    public bool isMelee = false;
    public bool isMultiShot = false;
    public bool isSpray = false;

    [Header("VFX & Audio")]
    public bool hasVFX = false;
    public bool hasSounds = false;

    [HideInInspector] public bool explodeOnImpact = true;
    [HideInInspector] public float explosionRange = 5.0f;
    [HideInInspector] public float explosionPower = 5.0f;
    [HideInInspector] public float fuseTimer = 5.0f;

    [HideInInspector] public float vfxSize = 1.0f;
    [HideInInspector] public float vfxDuration = 2.5f;
    [HideInInspector] public GameObject vfxObject = null;

    [HideInInspector] public bool lingerEffect = false;
    [HideInInspector] public float lingerEffectRange = 3.0f;
    [HideInInspector] public bool dealDamage = true;
    [HideInInspector] public int damagePerSecond = 5;
    [HideInInspector] public bool slowMovement = true;
    [HideInInspector] public float slowMultiplier = 0.5f;

    [HideInInspector] public float knockbackStrength = 2.0f;
    [HideInInspector] public float upwardsModifier = 4.0f;

    [HideInInspector] public int numOfShots = 2;
    [HideInInspector] public float delayBetweenShots = 0.5f;

    [HideInInspector] public float minimumRandomness = 0.8f;
    [HideInInspector] public float maximumRandomness = 1.2f;

    [HideInInspector] public float sprayLength = 3.0f;
    [HideInInspector] public float sprayHeight = 2.0f;
    [HideInInspector] public float sprayDuration = 2.5f;
    [HideInInspector] public float sprayAreaVFXSize = 1.0f;
    [HideInInspector] public GameObject sprayAreaObject = null;
    [HideInInspector] public GameObject sprayAreaVFX = null;

    [HideInInspector] public AudioClip shootSound;
    [HideInInspector] public AudioClip impactSound;
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
            baseWeapon.upwardsModifier = EditorGUILayout.FloatField("Updwards Modifier", baseWeapon.upwardsModifier);
            baseWeapon.explodeOnImpact = EditorGUILayout.Toggle("Explode On Impact", baseWeapon.explodeOnImpact);

            if (!baseWeapon.explodeOnImpact) {
                baseWeapon.fuseTimer = EditorGUILayout.FloatField("Fuse Timer", baseWeapon.fuseTimer);
            }
        }

        if (baseWeapon.hasVFX == true) {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("VFX Settings", EditorStyles.boldLabel);
            baseWeapon.vfxObject = (GameObject)EditorGUILayout.ObjectField("VFX", baseWeapon.vfxObject, typeof(GameObject), true);
            baseWeapon.vfxSize = EditorGUILayout.FloatField("VFX Size", baseWeapon.vfxSize);
            baseWeapon.vfxDuration = EditorGUILayout.FloatField("VFX Duration", baseWeapon.vfxDuration);

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Linger Effect Settings", EditorStyles.boldLabel);
            baseWeapon.lingerEffect = EditorGUILayout.Toggle("Linger Effect", baseWeapon.lingerEffect);

            if (baseWeapon.lingerEffect == true) {
                baseWeapon.lingerEffectRange = EditorGUILayout.FloatField("Linger Effect Range", baseWeapon.lingerEffectRange);
                baseWeapon.dealDamage = EditorGUILayout.Toggle("Deal Damage", baseWeapon.dealDamage);

                if (baseWeapon.dealDamage == true) {
                    baseWeapon.damagePerSecond = EditorGUILayout.IntField("Damage Per Second", baseWeapon.damagePerSecond);
                }

                baseWeapon.slowMovement = EditorGUILayout.Toggle("Slow Movement", baseWeapon.slowMovement);

                if (baseWeapon.slowMovement == true) {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("The closer to 0 the more the ant will be slowed.");
                    baseWeapon.slowMultiplier = EditorGUILayout.FloatField("Slow Multiplier", baseWeapon.slowMultiplier);
                }
            }
        }

        if (baseWeapon.isMelee == true) {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Melee Settings", EditorStyles.boldLabel);
            baseWeapon.knockbackStrength = EditorGUILayout.FloatField("Knockback Strength", baseWeapon.knockbackStrength);
            baseWeapon.upwardsModifier = EditorGUILayout.FloatField("Updwards Modifier", baseWeapon.upwardsModifier);
        }

        if (baseWeapon.isMultiShot == true) {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Multishot Settings", EditorStyles.boldLabel);
            baseWeapon.numOfShots = EditorGUILayout.IntField("Number Of Shots", baseWeapon.numOfShots);
            baseWeapon.delayBetweenShots = EditorGUILayout.FloatField("Delay Between Shots", baseWeapon.delayBetweenShots);
        }

        if (baseWeapon.hasSounds == true) {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Audio Settings", EditorStyles.boldLabel);
            baseWeapon.shootSound = (AudioClip)EditorGUILayout.ObjectField("Shoot Sound", baseWeapon.shootSound, typeof(AudioClip), true);
            baseWeapon.impactSound = (AudioClip)EditorGUILayout.ObjectField("Impact Sound", baseWeapon.impactSound, typeof(AudioClip), true);
        }

        if (baseWeapon.weaponRandomisation == true) {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Randomisation Settings", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Effects how much randomness is applied when the projectiles are shot.");
            EditorGUILayout.LabelField("A random value is chosen within the range selected.");
            baseWeapon.minimumRandomness = EditorGUILayout.FloatField("Minimum Randomness", baseWeapon.minimumRandomness);
            baseWeapon.maximumRandomness = EditorGUILayout.FloatField("Maximum Randomness", baseWeapon.maximumRandomness);
        }

        if (baseWeapon.isSpray == true) {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Spray Settings", EditorStyles.boldLabel);
            baseWeapon.sprayLength = EditorGUILayout.FloatField("Spray Length", baseWeapon.sprayLength);
            baseWeapon.sprayHeight = EditorGUILayout.FloatField("Spray Height", baseWeapon.sprayHeight);
            baseWeapon.sprayDuration = EditorGUILayout.FloatField("Spray Duration", baseWeapon.sprayDuration);
            baseWeapon.sprayAreaVFXSize = EditorGUILayout.FloatField("Spray Area VFX Size", baseWeapon.sprayAreaVFXSize);
            baseWeapon.sprayAreaObject = (GameObject)EditorGUILayout.ObjectField("Spray Area Prefab", baseWeapon.sprayAreaObject, typeof(GameObject), true);
            baseWeapon.sprayAreaVFX = (GameObject)EditorGUILayout.ObjectField("Spray Area VFX", baseWeapon.sprayAreaVFX, typeof(GameObject), true);
        }
    }
}
#endif