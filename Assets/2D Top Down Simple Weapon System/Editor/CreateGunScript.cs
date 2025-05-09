using UnityEditor;
using UnityEngine;

namespace WeaponSystem {
    public class CreateGunScript : EditorWindow {
        private bool useCustomGunName = false;
        private bool generateRandomGunName = true;
        private string gunName = string.Empty;
        private bool createRandomGun = false;
        private bool automaticReload = true;

        private WeaponType weaponType;
        private int burstCount = 3;
        private int minBurstCount = 1;
        private int maxBurstCount = 5;

        private bool useDelayBetweenShots = true;
        private float timeBetweenShots = 0.5f;
        private float minTimeBetweenShots = 0.05f;
        private float maxTimeBetweenShots = 1.5f;

        private bool useSetReloadTime = false;
        private float reloadTime = 1.5f;
        private float minReloadTime = 0.5f;
        private float maxReloadTime = 3f;

        private bool useAmmo = true;
        private int maxClipSize = 15;
        private int clipSizeMin = 5;
        private int clipSizeMax = 30;

        private bool useAudio = false;
        private AudioClip fireSound;
        private AudioClip dryFireSound;
        private AudioClip reloadSound;
        private AudioClip explosionSound;

        private enum WeaponType {
            Single,
            Automatic,
            Burst
        }

        private enum BulletPenetrationLevel {
            VeryLow,
            Low,
            Medium,
            High,
            VeryHigh
        }

        private float bulletSpeed = 20f;
        private float minBulletSpeed = 7.5f;
        private float maxBulletSpeed = 30f;
        private int bulletDamage = 10;
        private int minBulletDamage = 2;
        private int maxBulletDamage = 20;
        private float bulletLife = 2.5f;
        private float minBulletLife = 0.5f;
        private float maxBulletLife = 3.5f;
        private bool explosiveAmmo = false;
        private bool bulletPenetration = false;
        private GameObject impactParticlePrefab;
        private GameObject explosionParticlePrefab;
        private BulletPenetrationLevel bulletPenetrationLevel;

        Vector2 scrollPos;

        [MenuItem("Window/Simple Weapon System/Create New Gun %&g")] //%&g = cntrl + alt + g
        public static void OpenCreateGunMenu() {
            CreateGunScript window = GetWindow<CreateGunScript>();
            window.titleContent = new GUIContent("Create New Gun Menu");
        }

        private void OnEnable() {
            GenerateRandomGunName();
        }

        public void OnGUI() {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUIStyle bigLabelStyle = new() {
                fontStyle = FontStyle.BoldAndItalic,
                fontSize = 20
            };
            bigLabelStyle.normal.textColor = Color.white;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField("Create New Gun", bigLabelStyle);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("Gun Name", EditorStyles.boldLabel);
            useCustomGunName = EditorGUILayout.Toggle("Use Custom Gun Name", useCustomGunName);

            //Use a custom gun name or randomly generated name
            if (useCustomGunName == true) {
                generateRandomGunName = true;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Gun Name", GUILayout.MaxWidth(162));
                gunName = EditorGUILayout.TextField(gunName);
                EditorGUILayout.EndHorizontal();
            }
            else {
                GenerateRandomGunName();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Gun Name", GUILayout.MaxWidth(162));
                EditorGUILayout.LabelField(gunName);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Gun Settings", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Create Randomly Generated Gun", GUILayout.MaxWidth(200)); //Create guns procedurally or with set values
            createRandomGun = EditorGUILayout.Toggle(createRandomGun);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Ammo Settings", EditorStyles.boldLabel);
            useAmmo = EditorGUILayout.Toggle("Use Ammo Limit", useAmmo);
            useDelayBetweenShots = EditorGUILayout.Toggle("Use Delay Between Shots", useDelayBetweenShots);

            //Only display timeBetweenShots field if useDelayBetweenShots is true
            if (useDelayBetweenShots == true) {
                if (createRandomGun == true) {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Time Between Shots", EditorStyles.boldLabel);
                    minTimeBetweenShots = EditorGUILayout.FloatField("Min Time Between Shots", minTimeBetweenShots);
                    maxTimeBetweenShots = EditorGUILayout.FloatField("Max Time Between Shots", maxTimeBetweenShots);
                }
                else {
                    timeBetweenShots = EditorGUILayout.FloatField("Time Between Shots", timeBetweenShots);
                }
            }

            //Only display additional ammo related settings if there is an ammo limit
            if (useAmmo == true) {
                automaticReload = EditorGUILayout.Toggle("Automatic Reload", automaticReload);
                useSetReloadTime = EditorGUILayout.Toggle("Use Set Reload Time", useSetReloadTime);

                //Only display maxClipSize field if useAmmo is true
                if (createRandomGun == true) {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Clip Size", EditorStyles.boldLabel);
                    clipSizeMin = EditorGUILayout.IntField("Min Clip Size", clipSizeMin);
                    clipSizeMax = EditorGUILayout.IntField("Max Clip Size", clipSizeMax);
                }
                else {
                    maxClipSize = EditorGUILayout.IntField("Max Clip Size", maxClipSize);
                }

                //Only display reloadTime field if useSetReloadTime is true
                if (useSetReloadTime == true) {
                    if (createRandomGun == true) {
                        EditorGUILayout.Space(5);
                        EditorGUILayout.LabelField("Reload Time", EditorStyles.boldLabel);
                        minReloadTime = EditorGUILayout.FloatField("Min Reload Time", minReloadTime);
                        maxReloadTime = EditorGUILayout.FloatField("Max Reload Time", maxReloadTime);
                    }
                    else {
                        reloadTime = EditorGUILayout.FloatField("Reload Time", reloadTime);
                    }
                }
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Audio Settings", EditorStyles.boldLabel);
            useAudio = EditorGUILayout.Toggle("Use Audio", useAudio);

            //Display Audio Clips
            if (useAudio == true) {
                fireSound = EditorGUILayout.ObjectField("Fire Sound", fireSound, typeof(AudioClip), false) as AudioClip;
                dryFireSound = EditorGUILayout.ObjectField("Dry Fire Sound", dryFireSound, typeof(AudioClip), false) as AudioClip;
                reloadSound = EditorGUILayout.ObjectField("Reload Sound", reloadSound, typeof(AudioClip), false) as AudioClip;

                if (explosiveAmmo == true) {
                    explosionSound = EditorGUILayout.ObjectField("Explosion Sound", explosionSound, typeof(AudioClip), false) as AudioClip;
                }
            }

            if (createRandomGun == false) {
                EditorGUILayout.Space(15);
                EditorGUILayout.LabelField("Weapon Type", EditorStyles.boldLabel);
                weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", weaponType);

                //Set burst count if weapon is a burst weapon
                if (weaponType == WeaponType.Burst) {
                    if (createRandomGun == true) {
                        EditorGUILayout.Space(5);
                        EditorGUILayout.LabelField("Burst Count", EditorStyles.boldLabel);
                        minBurstCount = EditorGUILayout.IntField("Min Burst Count", minBurstCount);
                        maxBurstCount = EditorGUILayout.IntField("Max Burst Count", maxBurstCount);
                    }
                    else {
                        burstCount = EditorGUILayout.IntField("Burst Count", burstCount);
                    }
                }
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Bullet Settings", EditorStyles.boldLabel);

            //Bullet settings
            if (createRandomGun == true) {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Bullet Speed", EditorStyles.boldLabel);
                minBulletSpeed = EditorGUILayout.FloatField("Min Bullet Speed", minBulletSpeed);
                maxBulletSpeed = EditorGUILayout.FloatField("Max Bullet Speed", maxBulletSpeed);

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Bullet Damage", EditorStyles.boldLabel);
                minBulletDamage = EditorGUILayout.IntField("Min Bullet Damage", minBulletDamage);
                maxBulletDamage = EditorGUILayout.IntField("Max Bullet Damage", maxBulletDamage);

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Bullet Life", EditorStyles.boldLabel);
                minBulletLife = EditorGUILayout.FloatField("Min Bullet Life", minBulletLife);
                maxBulletLife = EditorGUILayout.FloatField("Max Bullet Life", maxBulletLife);
            }
            else {
                bulletSpeed = EditorGUILayout.FloatField("Bullet Speed", bulletSpeed);
                bulletDamage = EditorGUILayout.IntField("Bullet Damage", bulletDamage);
                bulletLife = EditorGUILayout.FloatField("Bullet Life", bulletLife);
            }

            explosiveAmmo = EditorGUILayout.Toggle("Explosive Ammo", explosiveAmmo);

            if (createRandomGun == false) {
                bulletPenetration = EditorGUILayout.Toggle("Bullet Penetration", bulletPenetration);
            }

            impactParticlePrefab = EditorGUILayout.ObjectField("Bullet Impact Particle Prefab", impactParticlePrefab, typeof(GameObject), false) as GameObject;

            if (explosiveAmmo == true) {
                explosionParticlePrefab = EditorGUILayout.ObjectField("Explosion Particle Prefab", explosionParticlePrefab, typeof(GameObject), false) as GameObject;
            }

            if (bulletPenetration == true && createRandomGun == false) {
                bulletPenetrationLevel = (BulletPenetrationLevel)EditorGUILayout.EnumPopup("Bullet Penetration Level", bulletPenetrationLevel);
            }

            EditorGUILayout.Space(25);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create New Gun", GUILayout.Width(250), GUILayout.Height(50))) {
                CreateGun();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        //Create new gun scriptable object
        private void CreateGun() {
            GunObject newGunSO = ScriptableObject.CreateInstance<GunObject>();

            if (createRandomGun == true) {
                timeBetweenShots = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
                newGunSO.timeBetweenShots = (float)System.Math.Round(timeBetweenShots, 2);

                reloadTime = Random.Range(minReloadTime, maxReloadTime);
                newGunSO.reloadTime = (float)System.Math.Round(reloadTime, 2);

                newGunSO.maxClipSize = Random.Range(clipSizeMin, clipSizeMax);
                newGunSO.burstCount = Random.Range(minBurstCount, maxBurstCount);

                bool bulletPen = Random.value < 0.5f;
                int randPenLevel = 0;

                if (bulletPen == true) {
                    randPenLevel = Random.Range(0, 5);
                }

                newGunSO.bulletSettings = new() {
                    bulletSpeed = (float)System.Math.Round(Random.Range(minBulletSpeed, maxBulletSpeed), 2),
                    bulletDamage = Random.Range(minBulletDamage, maxBulletDamage),
                    bulletLife = (float)System.Math.Round(Random.Range(minBulletLife, maxBulletLife), 2),
                    explosiveBullets = Random.value < 0.5f,
                    bulletPenetration = bulletPen,
                    bulletPenetrationLevel = (GunObject.BulletSettings.BulletPenetrationLevel)randPenLevel,
                    impactParticlePrefab = impactParticlePrefab,
                    explosionParticlePrefab = explosionParticlePrefab
                };
            }
            else {
                newGunSO.timeBetweenShots = timeBetweenShots;
                newGunSO.reloadTime = reloadTime;
                newGunSO.maxClipSize = maxClipSize;
                newGunSO.burstCount = burstCount;

                newGunSO.bulletSettings = new() {
                    bulletSpeed = bulletSpeed,
                    bulletDamage = bulletDamage,
                    bulletLife = bulletLife,
                    explosiveBullets = explosiveAmmo,
                    bulletPenetration = bulletPenetration,
                    bulletPenetrationLevel = (GunObject.BulletSettings.BulletPenetrationLevel)bulletPenetrationLevel,
                    impactParticlePrefab = impactParticlePrefab,
                    explosionParticlePrefab = explosionParticlePrefab
                };
            }

            newGunSO.gunName = gunName;
            newGunSO.useDelayBetweenShots = useDelayBetweenShots;
            newGunSO.automaticReload = automaticReload;
            newGunSO.useSetReloadTime = useSetReloadTime;
            newGunSO.useAmmoLimit = useAmmo;
            newGunSO.useAudio = useAudio;
            newGunSO.weaponType = (GunObject.WeaponType)weaponType;

            newGunSO.fireSound = fireSound;
            newGunSO.dryFireSound = dryFireSound;
            newGunSO.reloadSound = reloadSound;
            newGunSO.explosionSound = explosionSound;

            string assetPath = "Assets/2D Top Down Simple Weapon System/ScriptableObjects/" + gunName + ".asset"; //Path where the new gun will be created
            AssetDatabase.CreateAsset(newGunSO, assetPath);
        }

        //Generates a new random gun name
        private void GenerateRandomGunName() {
            if (generateRandomGunName == true) {
                generateRandomGunName = false;

                string gunNamesCSV = Application.dataPath + "/2D Top Down Simple Weapon System/Editor" + "/GunNames.csv";
                string[] gunNamesArray = System.IO.File.ReadAllText(gunNamesCSV).Split(",");
                gunName = gunNamesArray[Random.Range(0, gunNamesArray.Length)];
            }
        }
    }
}