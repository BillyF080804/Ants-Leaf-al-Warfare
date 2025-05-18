using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace WeaponSystem {
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovementScript : MonoBehaviour {
        [Header("Player Settings")]
        [SerializeField] private GameObject playerObject;
        [SerializeField] private GameObject playerSprite;
        [SerializeField] private float playerSpeed = 3;
        [SerializeField] private float maxRotateSpeed = 360;
        public bool useSprinting = true;

        private Vector2 playerMoveAxis;
        private bool playerSprinting;
        private bool playerAiming;
        private float angle;
        private float currentRotationVelocity;
        [HideInInspector] public float sprintMultiplier = 2;

        [Header("Camera Settings")]
        public bool useCameraFollowScript = true;
        public bool useAiming = true;
        [HideInInspector] public CameraFollowScript cameraFollowScript;
        [HideInInspector] public float aimMultiplier = 2;

        [Header("Gun Settings")]
        [SerializeField] private GunScript gunScript;
        private bool shootingGun;
        public bool overrideGunScriptGunObject;
        [HideInInspector] public GunObject gunObjectOverride;

        [Header("Player Health")]
        [SerializeField] private float playerHealth = 100;
        private bool isTouchingEnemy = false;
        private float damageTimer = 0.0f;

        [Header("Fade Scripts")]
        [SerializeField] private FadeScript blackscreenFadeScript;
        [SerializeField] private FadeScript damageFadeScript;

        //Override the selected gunObject if applicable
        private void Awake() {
            if (gunScript != null && overrideGunScriptGunObject == true && gunObjectOverride != null) {
                gunScript.gunObject = gunObjectOverride;
            }

            blackscreenFadeScript.gameObject.SetActive(true);
            blackscreenFadeScript.FadeOutUI(1.0f);
        }

        //Check values and assign default values if required
        private void Start() {
            if (gunScript == null) {
                Debug.LogWarning("PlayerMovementScript did not have an assigned gunScript. Attempting to find suitable replacement.");
                gunScript = FindFirstObjectByType<GunScript>();
            }

            if (useCameraFollowScript == true && cameraFollowScript == null) {
                Debug.LogWarning("PlayerMovementScript did not have an assigned cameraFollowScript. Attempting to find suitable replacement.");
                cameraFollowScript = FindFirstObjectByType<CameraFollowScript>();
            }
        }

        private void Update() {
            //Player Movement
            playerObject.transform.Translate(playerSpeed * Time.deltaTime * (Vector3)playerMoveAxis);
            PlayerRotation();

            if (isTouchingEnemy == true) {
                damageTimer += Time.deltaTime;

                if (damageTimer >= 1.0f) {
                    playerHealth -= 20;
                    damageFadeScript.gameObject.SetActive(true);
                    damageFadeScript.FadeOutUI(0.25f);

                    if (playerHealth <= 0) {
                        StartCoroutine(ReturnToMainMenuCoroutine());
                    }
                    else {
                        damageTimer = 0.0f;
                    }
                }
            }
        }

        private IEnumerator ReturnToMainMenuCoroutine() {
            blackscreenFadeScript.gameObject.SetActive(true);
            blackscreenFadeScript.FadeInUI(1.0f);
            yield return new WaitForSeconds(1.1f);
            SceneManager.LoadScene("MainMenu");
        }

        //Handles the player rotation
        private void PlayerRotation() {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 direction = mousePosition - playerObject.transform.position;
            float targetAngle = Vector2.SignedAngle(Vector2.right, direction);

            angle = Mathf.SmoothDampAngle(angle, targetAngle, ref currentRotationVelocity, 0.3f, maxRotateSpeed);
            playerSprite.transform.eulerAngles = new Vector3(0, 0, angle);
        }

        private void OnMove(InputValue value) {
            playerMoveAxis = value.Get<Vector2>();
        }

        //Increases the player speed & zooms camera out
        private void OnSprint() {
            if (useSprinting == true) {
                playerSprinting = !playerSprinting;

                if (playerSprinting) {
                    playerSpeed *= sprintMultiplier;
                }
                else {
                    playerSpeed /= sprintMultiplier;
                }

                if (cameraFollowScript != null) {
                    cameraFollowScript.SetPlayerSprintingCamSize(playerSprinting);
                }
            }
        }

        //Slows the player speed & zooms camera in
        private void OnAim() {
            if (useAiming == true) {
                playerAiming = !playerAiming;

                if (playerAiming) {
                    playerSpeed /= aimMultiplier;
                }
                else {
                    playerSpeed *= aimMultiplier;
                }

                if (cameraFollowScript != null) {
                    cameraFollowScript.SetPlayerZoomingCam(gunScript);
                }
            }
        }

        //Calls the trigger on the weapon
        private void OnShoot() {
            shootingGun = !shootingGun;

            gunScript.TriggerWeapon(shootingGun);
        }
         
        //Used for manual reloads
        private void OnReload() {
            gunScript.ManualReload();
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.collider.CompareTag("Enemy")) {
                isTouchingEnemy = true;                
            }
        }

        private void OnCollisionStay2D(Collision2D collision) {
            if (collision.collider.CompareTag("Enemy")) {
                isTouchingEnemy = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision) {
            if (collision.collider.CompareTag("Enemy")) {
                isTouchingEnemy = false;
            }
        }
    }

    //Custom editor
#if (UNITY_EDITOR)
    [CustomEditor(typeof(PlayerMovementScript))]
    public class PlayerMovementEditor : Editor
    {
        [InitializeOnEnterPlayMode]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PlayerMovementScript movementScript = (PlayerMovementScript)target;

            //Only display field if required
            if (movementScript.useCameraFollowScript == true)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Camera Follow Script", EditorStyles.boldLabel);

                movementScript.cameraFollowScript = EditorGUILayout.ObjectField("Camera Follow Script", movementScript.cameraFollowScript, typeof(CameraFollowScript), true) as CameraFollowScript;
            }

            //Only display field if required
            if (movementScript.useSprinting == true)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Sprint Settings", EditorStyles.boldLabel);

                movementScript.sprintMultiplier = EditorGUILayout.FloatField("Sprint Speed Multiplier", movementScript.sprintMultiplier);
            }

            //Only display field if required
            if (movementScript.useAiming == true)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Aim Settings", EditorStyles.boldLabel);

                movementScript.aimMultiplier = EditorGUILayout.FloatField("Aim Speed Multiplier", movementScript.aimMultiplier);
            }

            //Only display field if required
            if (movementScript.overrideGunScriptGunObject == true) {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Gun Object", EditorStyles.boldLabel);

                EditorGUILayout.LabelField("WARNING! This will override the selected gun on the GunScript.\nUntoggle 'Override Gun Script Gun Object' if you do not wish to do this.", GUILayout.Height(50));

                GUIContent gunObjectContent = new GUIContent("Gun Object", "WARNING! This will override the selected gun on the GunScript.");
                movementScript.gunObjectOverride = EditorGUILayout.ObjectField(gunObjectContent, movementScript.gunObjectOverride, typeof(GunObject), false) as GunObject;
            }
        }
    }
#endif
}