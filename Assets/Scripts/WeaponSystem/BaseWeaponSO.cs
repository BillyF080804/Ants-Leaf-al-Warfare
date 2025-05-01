using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon System/New Weapon", fileName = "New Weapon")]
public class BaseWeaponSO : ScriptableObject {
    [Header("Weapon Info")]
    public string weaponName;
    public string weaponDescription;
    public KnockbackLevels knockbackLevel;
    public bool unlimitedUses = true;

    public enum KnockbackLevels {
        Low,
        Medium,
        High
    }

    [Header("Weapon Cosmetics")]
    public bool useAutomaticRotation = true;
    public Sprite weaponIcon = null;
    public GameObject weaponPrefab = null;

    [Header("In-World Weapon")]
    public GameObject worldWeaponPrefab = null;
    public float worldWeaponScale = 1.0f;
    public Vector3 worldWeaponLocalPos = Vector3.zero;
    public Vector3 worldWeaponRotation = Vector3.zero;

    [Header("Weapon Settings")]
    public int baseDamage = 20;
    public float weaponSpeed = 20.0f;
    public bool useGravity = true;
    [Tooltip("This will add some variance onto the speed & aim of the weapon when shot.")] public bool weaponRandomisation = false;
    public bool explosive = false;
    public bool hasKnockback = false;
    public bool isMelee = false;
    public bool isMultiShot = false;
    public bool isSpray = false;

    [Header("Weapon Effect - Leave empty for no effect")]
    public GameObject weaponEffect = null;

    [Header("Camera Settings")]
    public float cameraDelay = 4.0f;
    public bool cameraShakeOnFire = false;
    public bool cameraShakeOnImpact = false;

    [Header("VFX & Audio")]
    public bool hasVFX = false;
    public bool hasSounds = false;

    [HideInInspector] public bool explodeOnImpact = true;
    [HideInInspector] public float explosionRange = 5.0f;
    [HideInInspector] public float explosionPower = 10.0f;
    [HideInInspector] public float fuseTimer = 5.0f;
    [HideInInspector] public int maxNumOfBounces = 3;

    [HideInInspector] public float vfxSize = 1.0f;
    [HideInInspector] public float vfxDuration = 2.5f;
    [HideInInspector] public GameObject vfxObject = null;

    [HideInInspector] public float knockbackStrength = 10.0f;
    [HideInInspector] public float upwardsModifier = 3.0f;

    [HideInInspector] public int numOfShots = 2;
    [HideInInspector] public float delayBetweenShots = 0.5f;

    [HideInInspector] public float minimumRandomness = 0.8f;
    [HideInInspector] public float maximumRandomness = 1.2f;

    [HideInInspector] public bool sprayMover = false;
    [HideInInspector] public float sprayStrength = 25.0f;
    [HideInInspector] public float sprayLength = 3.0f;
    [HideInInspector] public float sprayHeight = 2.0f;
    [HideInInspector] public float sprayDuration = 2.5f;
    [HideInInspector] public float sprayAreaVFXSize = 1.0f;
    [HideInInspector] public GameObject sprayAreaObject = null;
    [HideInInspector] public GameObject sprayAreaVFX = null;

    [HideInInspector] public float cameraShakeDuration = 0.25f;
    [HideInInspector] public float cameraShakeIntensity = 1;

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

        if (baseWeapon.explosive == true) {
            if (baseWeapon.hasKnockback == true) {
                baseWeapon.hasKnockback = false;
                Debug.LogWarning("Disabled Knockback. Weapon Cannot Have Knockback and Explosive enabled at the same time.\nExplosive weapons have their own knockback settings.");
            }
            else if (baseWeapon.isSpray == true) {
                baseWeapon.isSpray = false;
                Debug.LogWarning("Disabled Spray. Weapon Cannot Have Spray and Explosive enabled at the same time.\nSpray weapons have their own knockback settings.");
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Explosion Settings", EditorStyles.boldLabel);
            baseWeapon.explosionRange = EditorGUILayout.FloatField("Explosion Range", baseWeapon.explosionRange);
            baseWeapon.explosionPower = EditorGUILayout.FloatField("Explosion Power", baseWeapon.explosionPower);
            baseWeapon.upwardsModifier = EditorGUILayout.FloatField("Upwards Modifier", baseWeapon.upwardsModifier);
            baseWeapon.explodeOnImpact = EditorGUILayout.Toggle("Explode On Impact", baseWeapon.explodeOnImpact);

            if (!baseWeapon.explodeOnImpact) {
                baseWeapon.fuseTimer = EditorGUILayout.FloatField("Fuse Timer", baseWeapon.fuseTimer);
            }
        }
        else {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Collision Settings", EditorStyles.boldLabel);
            baseWeapon.maxNumOfBounces = EditorGUILayout.IntField("Max Num Of Bounces", baseWeapon.maxNumOfBounces);
        }

        if (baseWeapon.hasKnockback == true || baseWeapon.isMelee == true) {
            if (baseWeapon.isSpray == true && baseWeapon.hasKnockback == true) {
                baseWeapon.isSpray = false;
                Debug.LogWarning("Disabled Spray. Weapon Cannot Have Spray and Knockback enabled at the same time.\nSpray weapons have their own knockback settings.");
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Knockback Settings", EditorStyles.boldLabel);
            baseWeapon.knockbackStrength = EditorGUILayout.FloatField("Knockback Strength", baseWeapon.knockbackStrength);
            baseWeapon.upwardsModifier = EditorGUILayout.FloatField("Updwards Modifier", baseWeapon.upwardsModifier);
        }

        if (baseWeapon.hasVFX == true) {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("VFX Settings", EditorStyles.boldLabel);
            baseWeapon.vfxObject = (GameObject)EditorGUILayout.ObjectField("VFX", baseWeapon.vfxObject, typeof(GameObject), true);
            baseWeapon.vfxSize = EditorGUILayout.FloatField("VFX Size", baseWeapon.vfxSize);
            baseWeapon.vfxDuration = EditorGUILayout.FloatField("VFX Duration", baseWeapon.vfxDuration);
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
            baseWeapon.sprayMover = EditorGUILayout.Toggle("Spray Moves Ant", baseWeapon.sprayMover);

            if (baseWeapon.sprayMover == true) {
                baseWeapon.sprayStrength = EditorGUILayout.FloatField("Spray Strength", baseWeapon.sprayStrength);
            }

            baseWeapon.sprayLength = EditorGUILayout.FloatField("Spray Length", baseWeapon.sprayLength);
            baseWeapon.sprayHeight = EditorGUILayout.FloatField("Spray Height", baseWeapon.sprayHeight);
            baseWeapon.sprayDuration = EditorGUILayout.FloatField("Spray Duration", baseWeapon.sprayDuration);
            baseWeapon.sprayAreaVFXSize = EditorGUILayout.FloatField("Spray Area VFX Size", baseWeapon.sprayAreaVFXSize);
            baseWeapon.sprayAreaObject = (GameObject)EditorGUILayout.ObjectField("Spray Area Prefab", baseWeapon.sprayAreaObject, typeof(GameObject), true);
            baseWeapon.sprayAreaVFX = (GameObject)EditorGUILayout.ObjectField("Spray Area VFX", baseWeapon.sprayAreaVFX, typeof(GameObject), true);
        }

        if (baseWeapon.cameraShakeOnFire || baseWeapon.cameraShakeOnImpact) {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Camera Shake Settings", EditorStyles.boldLabel);
            baseWeapon.cameraShakeDuration = EditorGUILayout.FloatField("Camera Shake Duration", baseWeapon.cameraShakeDuration);
            baseWeapon.cameraShakeIntensity = EditorGUILayout.FloatField("Camera Shake Intensity", baseWeapon.cameraShakeIntensity);
        }
    }
}
#endif