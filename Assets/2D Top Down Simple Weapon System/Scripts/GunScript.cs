using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace WeaponSystem {
    public class GunScript : MonoBehaviour {
        [Header("Reticle Settings")]
        [SerializeField] private GameObject reticle;
        [SerializeField] private Color reticleColor = Color.white;

        [Header("Gun Settings")]
        public GunObject gunObject;
        [SerializeField] private GameObject bulletPrefab;
        private bool triggerHeld;
        private bool canFire = true;
        private bool isReloading;
        private int currentAmmo;

        [Header("Player")]
        [SerializeField] private GameObject player;

        [Header("UI Settings")]
        public bool enableUI;

        [HideInInspector] public TMP_Text ammoText;
        [HideInInspector] public TMP_Text gunName;
        [HideInInspector] public AudioSource audioSource;

        private BulletSpawner bulletSpawner;

        //Gets default values and if required sets them
        private void Start() {
            if (reticle == null) {
                Debug.LogWarning("GunScript did not have an assigned reticle. Attempting to find suitable replacement.");
                reticle = gameObject;
            }

            if (player == null) {
                Debug.LogWarning("GunScript did not have an assigned player. Attempting to find suitable replacement.");
                player = FindFirstObjectByType<PlayerMovementScript>().gameObject;
            }

            if (bulletPrefab == null) {
                Debug.LogError("ERROR. GunScript does not have a bulletPrefab. Please assign one.");
            }

            if (gunObject == null) {
                Debug.LogError("ERROR. GunScript does not have a gunObject. Please assign one.");
            }
            else if (gunObject != null) {
                currentAmmo = gunObject.maxClipSize;

                if (gunObject.useAudio == true && audioSource == null) {
                    Debug.LogWarning("GunScript did not have an assigned audioSource. Attempting to find suitable replacement.");

                    //Adds a new audio source if one is not found or assigns the audio source on the gameobject
                    if (!gameObject.TryGetComponent(out audioSource)) {
                        audioSource = gameObject.AddComponent<AudioSource>();
                        audioSource.playOnAwake = false;
                    }
                }

                if (enableUI == true) {
                    if (ammoText == null) {
                        Debug.LogWarning("GunScript did not have an assigned ammoText. Please Assign One.");
                    }
                    else {
                        if (gunObject.useAmmoLimit == false) {
                            ammoText.text = "infAmmo";
                        }
                        else {
                            ammoText.text = gunObject.maxClipSize + " / " + gunObject.maxClipSize;
                        }
                    }

                    if (gunName == null) {
                        Debug.LogWarning("GunScript did not have an assigned gunName. Please Assign One.");
                    }
                    else {
                        gunName.text = gunObject.gunName;
                    }
                }
            }

            bulletSpawner = GetComponent<BulletSpawner>();
            reticle.GetComponent<SpriteRenderer>().color = reticleColor; //Set reticle color
        }

        //Gets the transform for the reticle
        public Transform GetReticleTransform() {
            return reticle.transform;
        }

        //Return bullet prefab
        public GameObject GetBulletPrefab() {
            return bulletPrefab;
        }

        //Return player transform
        public Transform GetPlayerTransform() {
            return player.transform;
        }

        //Called when the player presses the "Shoot" button
        public void TriggerWeapon(bool heldTrigger) {
            triggerHeld = heldTrigger;

            if (triggerHeld == true && isReloading == false) {
                if (gunObject.weaponType == GunObject.WeaponType.Single) {
                    StartCoroutine(SingleShot());
                }
                else if (gunObject.weaponType == GunObject.WeaponType.Automatic) {
                    StartCoroutine(AutomaticFire());
                }
                else if (gunObject.weaponType == GunObject.WeaponType.Burst) {
                    StartCoroutine(BurstFire());
                }
            }
        }

        //Handles functionality single shot weapons
        private IEnumerator SingleShot() {
            if (canFire) {
                canFire = false;

                if (gunObject.useAmmoLimit) {
                    if (currentAmmo > 0) {
                        //Fire gun and play audio
                        if (gunObject.useAudio == true && audioSource != null) {
                            audioSource.clip = gunObject.fireSound;
                            audioSource.Play();
                        }

                        FireBullet();
                    }
                    else if (currentAmmo == 0 && gunObject.automaticReload == false) {
                        //Play dry fire sound - player has no ammo
                        if (gunObject.useAudio == true && audioSource != null) {
                            audioSource.clip = gunObject.dryFireSound;
                            audioSource.Play();
                        }
                    }
                    else if (currentAmmo == 0 && gunObject.automaticReload == true) {
                        StartCoroutine(ReloadGun()); //Reload the gun
                    }
                }
                else if (gunObject.useAmmoLimit == false) {
                    //Fire gun and play audio
                    if (gunObject.useAudio == true && audioSource != null) {
                        audioSource.clip = gunObject.fireSound;
                        audioSource.Play();
                    }

                    FireBullet();
                }

                if (gunObject.useDelayBetweenShots) {
                    yield return new WaitForSeconds(gunObject.timeBetweenShots);
                }
                else {
                    yield return null;
                }

                canFire = true;
            }
        }

        //Handles functionality for automatic weapons
        private IEnumerator AutomaticFire() {
            while (triggerHeld) {
                yield return null;

                if (canFire) {
                    canFire = false;

                    if (gunObject.useAmmoLimit) {
                        if (currentAmmo > 0) {
                            //Fire gun and play audio
                            if (gunObject.useAudio == true && audioSource != null) {
                                audioSource.clip = gunObject.fireSound;
                                audioSource.Play();
                            }

                            FireBullet();
                        }
                        else if (currentAmmo == 0 && gunObject.automaticReload == false) {
                            //Play dry fire sound - player has no ammo
                            if (gunObject.useAudio == true && audioSource != null) {
                                audioSource.clip = gunObject.dryFireSound;
                                audioSource.Play();
                            }
                        }
                        else if (currentAmmo == 0 && gunObject.automaticReload == true) {
                            StartCoroutine(ReloadGun()); //Reload the gun
                        }
                    }
                    else if (gunObject.useAmmoLimit == false) {
                        //Fire gun and play audio
                        if (gunObject.useAudio == true && audioSource != null) {
                            audioSource.clip = gunObject.fireSound;
                            audioSource.Play();
                        }

                        FireBullet();
                    }

                    if (gunObject.useDelayBetweenShots) {
                        yield return new WaitForSeconds(gunObject.timeBetweenShots);
                    }
                    else {
                        yield return null;
                    }

                    canFire = true;
                }
            }
        }

        //Handles functionality for burst fire weapons
        private IEnumerator BurstFire() {
            int burstCount = 0;

            while (burstCount < gunObject.burstCount) {
                burstCount++;

                if (canFire) {
                    canFire = false;

                    if (gunObject.useAmmoLimit) {
                        if (currentAmmo > 0) {
                            //Fire gun and play audio
                            if (gunObject.useAudio == true && audioSource != null) {
                                audioSource.clip = gunObject.fireSound;
                                audioSource.Play();
                            }

                            FireBullet();
                        }
                        else if (currentAmmo == 0 && gunObject.automaticReload == false) {
                            //Play dry fire sound - player has no ammo
                            if (gunObject.useAudio == true && audioSource != null) {
                                audioSource.clip = gunObject.dryFireSound;
                                audioSource.Play();
                            }
                        }
                        else if (currentAmmo == 0 && gunObject.automaticReload == true) {
                            StartCoroutine(ReloadGun()); //Reload the gun
                        }
                    }
                    else if (gunObject.useAmmoLimit == false) {
                        //Fire gun and play audio
                        if (gunObject.useAudio == true && audioSource != null) {
                            audioSource.clip = gunObject.fireSound;
                            audioSource.Play();
                        }
                        
                        FireBullet();
                    }

                    if (gunObject.useDelayBetweenShots) {
                        yield return new WaitForSeconds(gunObject.timeBetweenShots);
                    }
                    else {
                        yield return null;
                    }

                    canFire = true;
                }
            }
        }

        //Handles functionality for when a bullet is shot
        private void FireBullet() {
            bulletSpawner.bulletPool.Get(); //Spawn new bullet using object pool

            if (gunObject.useAmmoLimit) {
                currentAmmo--;
            }

            if (enableUI == true && ammoText != null && gunObject.useAmmoLimit == true) {
                ammoText.text = currentAmmo + " / " + gunObject.maxClipSize; //Update ammo text
            }

            player.TryGetComponent(out PlayerMovementScript playerMovementScript);

            if (playerMovementScript != null && playerMovementScript.cameraFollowScript != null && playerMovementScript.cameraFollowScript.useCameraShake == true) {
                playerMovementScript.cameraFollowScript.StartCameraShake(); //Shake camera
            }
        }

        //Reloads the gun
        private IEnumerator ReloadGun() {
            if (isReloading == false) {
                isReloading = true;

                if (enableUI == true && ammoText != null) {
                    ammoText.text = "Reloading...";
                }

                if (gunObject.useAudio == true) {
                    audioSource.clip = gunObject.reloadSound;
                    audioSource.Play();

                    yield return new WaitUntil(() => audioSource.isPlaying == false);
                }
                else {
                    yield return new WaitForSeconds(gunObject.reloadTime);
                }

                currentAmmo = gunObject.maxClipSize;
                isReloading = false;

                if (enableUI == true && ammoText != null) {
                    ammoText.text = currentAmmo + " / " + gunObject.maxClipSize;
                }
            }
        }

        //Function to allow the player to manually reload the gun. Called from the PlayerMovementScript.
        public void ManualReload() {
            StartCoroutine(ReloadGun());
        }
    }

    //Custom Editor
#if (UNITY_EDITOR)
    [CustomEditor(typeof(GunScript))]
    public class GunScriptEditor : Editor
    {
        [InitializeOnEnterPlayMode]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GunScript gunScript = (GunScript)target;

            if (gunScript.enableUI == true)
            {
                gunScript.ammoText = EditorGUILayout.ObjectField("Ammo Text", gunScript.ammoText, typeof(TMP_Text), true) as TMP_Text;
            }

            if (gunScript.enableUI == true)
            {
                gunScript.gunName = EditorGUILayout.ObjectField("Gun Name", gunScript.gunName, typeof(TMP_Text), true) as TMP_Text;
            }

            if (gunScript.gunObject != null)
            {
                if (gunScript.gunObject.useAudio == true)
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField("Audio Source", EditorStyles.boldLabel);

                    gunScript.audioSource = EditorGUILayout.ObjectField("Audio Source", gunScript.audioSource, typeof(AudioSource), true) as AudioSource;
                }
            }
        }
    }
#endif
}