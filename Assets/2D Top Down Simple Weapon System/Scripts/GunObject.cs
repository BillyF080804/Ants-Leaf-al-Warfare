using System;
using UnityEditor;
using UnityEngine;
using static WeaponSystem.GunObject;

namespace WeaponSystem {
    [CreateAssetMenu(menuName = "Simple Weapon System/New Gun", order = 1, fileName = "New Gun")]
    public class GunObject : ScriptableObject {
        [Header("Gun Name")]
        public string gunName;

        [Header("Settings")]
        public bool useDelayBetweenShots = true;
        public bool useAmmoLimit = false;
        public bool useAudio = false;

        public WeaponType weaponType;

        [HideInInspector] public int burstCount = 3;
        [HideInInspector] public float timeBetweenShots = 0.5f;
        [HideInInspector] public int maxClipSize = 15;
        [HideInInspector] public float reloadTime = 1.5f;

        [HideInInspector] public AudioClip fireSound;
        [HideInInspector] public AudioClip dryFireSound;
        [HideInInspector] public AudioClip reloadSound;
        [HideInInspector] public AudioClip explosionSound;
        [HideInInspector] public bool useSetReloadTime = false;
        [HideInInspector] public bool automaticReload = true;

        public enum WeaponType {
            Single,
            Automatic,
            Burst
        }

        [Header("Bullet Settings")]
        public BulletSettings bulletSettings;

        [Serializable]
        public class BulletSettings
        {
            public float bulletSpeed = 20f;
            public int bulletDamage = 10;
            public float bulletLife = 2.5f;
            public bool explosiveBullets;
            public bool bulletPenetration;

            [HideInInspector] public GameObject impactParticlePrefab;
            [HideInInspector] public GameObject explosionParticlePrefab;
            [HideInInspector] public BulletPenetrationLevel bulletPenetrationLevel;

            public enum BulletPenetrationLevel
            {
                VeryLow,
                Low,
                Medium,
                High,
                VeryHigh
            }
        }
    }

    //Custom editor
#if (UNITY_EDITOR)
    [CustomEditor(typeof(GunObject))]
    public class GunObjectEditor : Editor
    {
        [InitializeOnEnterPlayMode]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GunObject gunObject = (GunObject)target;

            //Only display burstCount field if the weaponType is burst
            if (gunObject.weaponType == WeaponType.Burst)
            {
                EditorGUILayout.Space(15);
                EditorGUILayout.LabelField("Burst Count", EditorStyles.boldLabel);

                gunObject.burstCount = EditorGUILayout.IntField("Burst Count", gunObject.burstCount);
            }

            //Only display timeBetweenShots field if useDelayBetweenShots is true
            if (gunObject.useDelayBetweenShots == true)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Time Between Shots", EditorStyles.boldLabel);
                gunObject.timeBetweenShots = EditorGUILayout.FloatField("Time Between Shots", gunObject.timeBetweenShots);
            }

            //Only display maxClipSize field if useAmmo is true
            if (gunObject.useAmmoLimit == true)
            {
                EditorGUILayout.Space(15);
                EditorGUILayout.LabelField("Ammo Settings", EditorStyles.boldLabel);

                gunObject.automaticReload = EditorGUILayout.Toggle("Automatic Reload", gunObject.automaticReload);
                gunObject.useSetReloadTime = EditorGUILayout.Toggle("Use Set Reload Time", gunObject.useSetReloadTime);
                gunObject.useDelayBetweenShots = EditorGUILayout.Toggle("Use Delay Between Shots", gunObject.useDelayBetweenShots);
                gunObject.maxClipSize = EditorGUILayout.IntField("Max Clip Size", gunObject.maxClipSize);

                //Only display reloadTime field if useSetReloadTime is true
                if (gunObject.useSetReloadTime == true)
                {
                    gunObject.reloadTime = EditorGUILayout.FloatField("Reload Time", gunObject.reloadTime);
                }
            }

            //Only display audio clips if useAudio is true
            if (gunObject.useAudio == true)
            {
                EditorGUILayout.Space(15);
                EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);

                gunObject.fireSound = EditorGUILayout.ObjectField("Fire Sound", gunObject.fireSound, typeof(AudioClip), false) as AudioClip;

                gunObject.dryFireSound = EditorGUILayout.ObjectField("Dry Fire Sound", gunObject.dryFireSound, typeof(AudioClip), false) as AudioClip;

                gunObject.reloadSound = EditorGUILayout.ObjectField("Reload Sound", gunObject.reloadSound, typeof(AudioClip), false) as AudioClip;

                if (gunObject.bulletSettings.explosiveBullets == true) {
                    gunObject.explosionSound = EditorGUILayout.ObjectField("Explosion Sound", gunObject.explosionSound, typeof(AudioClip), false) as AudioClip;
                }
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Additional Bullet Settings", EditorStyles.boldLabel);

            gunObject.bulletSettings.impactParticlePrefab = EditorGUILayout.ObjectField("Bullet Impact Particle Prefab", gunObject.bulletSettings.impactParticlePrefab, typeof(GameObject), false) as GameObject;

            if (gunObject.bulletSettings.explosiveBullets == true)
            {
                gunObject.bulletSettings.explosionParticlePrefab = EditorGUILayout.ObjectField("Explosion Particle Prefab", gunObject.bulletSettings.explosionParticlePrefab, typeof(GameObject), false) as GameObject;
            }

            if (gunObject.bulletSettings.bulletPenetration == true)
            {
                gunObject.bulletSettings.bulletPenetrationLevel = (BulletSettings.BulletPenetrationLevel)EditorGUILayout.EnumPopup("Bullet Penetration Level", gunObject.bulletSettings.bulletPenetrationLevel);
            }
        }
    }
#endif
}