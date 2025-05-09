using System.Collections;
using UnityEditor;
using UnityEngine;

namespace WeaponSystem {
    public class CameraFollowScript : MonoBehaviour {
        [Header("Camera Settings")]
        [SerializeField] private Transform playerCameraObject;
        [SerializeField] private Transform playerCamera;
        public bool useCameraShake;
        [SerializeField] private float cameraSmoothTime = 0.3f;
        [SerializeField][Tooltip("Distance is only set on Awake (ie. entering play mode)")] private float cameraDistance = 5f;
        [HideInInspector] public bool explosionCameraShake;
        [HideInInspector] public float cameraShakeIntensity = 1.0f;

        private Vector3 cameraVelocity = Vector3.zero;
        private Vector3 cameraTargetPos = Vector3.zero;
        private bool playerAiming = false;
        private GunScript gunScript;
        private float endCamSize;
        private float startCamSize;

        [Header("Player Object")]
        [SerializeField] private GameObject playerObject;

        //Get default values and sets them if required
        private void Start() {
            if (playerCamera == null) {
                Debug.LogWarning("CameraFollowScript did not have an assigned playerCamera. Attempting to find suitable replacement.");
                playerCamera = FindFirstObjectByType<Camera>().transform;
            }

            if (playerCameraObject == null) {
                Debug.LogWarning("CameraFollowScript did not have an assigned playerCameraObject. Attempting to find suitable replacement.");
                playerCameraObject = transform;
            }

            if (playerObject == null) {
                Debug.LogWarning("CameraFollowScript did not have an assigned playerObject. Attempting to find suitable replacement.");
                playerObject = FindFirstObjectByType<PlayerMovementScript>().gameObject;
            }

            playerCamera.GetComponent<Camera>().orthographicSize = cameraDistance;
            endCamSize = cameraDistance;
            startCamSize = cameraDistance;
        }

        private void Update() {
            //Camera Movement & Smoothing
            if (playerAiming == true) {
                cameraTargetPos = new Vector3(gunScript.GetReticleTransform().position.x, gunScript.GetReticleTransform().position.y, -10); //Focus on reticle
            }
            else {
                cameraTargetPos = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, -10); //Focus on player
            }

            playerCameraObject.position = Vector3.SmoothDamp(playerCameraObject.position, cameraTargetPos, ref cameraVelocity, cameraSmoothTime);
            CameraZoom();
        }

        //Camera zoom functionality
        private void CameraZoom() {
            playerCamera.GetComponent<Camera>().orthographicSize = Mathf.MoveTowards(playerCamera.GetComponent<Camera>().orthographicSize, endCamSize, 3 * Time.deltaTime);
        }

        //Change the "FOV" when the player is sprinting
        public void SetPlayerSprintingCamSize(bool playerSprinting) {
            if (playerSprinting) {
                endCamSize = startCamSize + 1;
            }
            else {
                endCamSize = startCamSize;
            }
        }

        //Change the camera focus when the player is aiming
        public void SetPlayerZoomingCam(GunScript _gunScript) {
            playerAiming = !playerAiming;
            gunScript = _gunScript;
        }

        //Used to call the camera shake coroutine
        public void StartCameraShake() {
            StartCoroutine(CameraShake());
        }

        //Shakes the camera
        private IEnumerator CameraShake() {
            for (int i = 0; i < 2; i++) {
                float timeElapsed = 0;
                Vector3 startPos = playerCamera.transform.localPosition;
                Vector3 endPos = Vector3.zero + new Vector3(Random.Range(-0.1f * cameraShakeIntensity, 0.1f * cameraShakeIntensity), Random.Range(-0.1f * cameraShakeIntensity, 0.1f * cameraShakeIntensity), 0);

                if (i == 1) {
                    endPos = Vector3.zero;
                }

                while (timeElapsed < 0.1f) {
                    playerCamera.transform.localPosition = Vector3.Lerp(startPos, endPos, timeElapsed / 0.1f);
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }

                playerCamera.transform.localPosition = endPos;
                yield return new WaitUntil(() => playerCamera.transform.localPosition == endPos);
            }
        }
    }

    //Custom editor
#if (UNITY_EDITOR)
    [CustomEditor(typeof(CameraFollowScript))]
    public class CameraFollowEditor : Editor {
        [InitializeOnEnterPlayMode]
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            CameraFollowScript cameraFollowScript = (CameraFollowScript)target;

            //Only display field if required
            if (cameraFollowScript.useCameraShake == true) {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Additional Settings", EditorStyles.boldLabel);

                cameraFollowScript.explosionCameraShake = EditorGUILayout.Toggle("Explosion Camera Shake", cameraFollowScript.explosionCameraShake);
                cameraFollowScript.cameraShakeIntensity = EditorGUILayout.FloatField("Camera Shake Intensity", cameraFollowScript.cameraShakeIntensity);
            }
        }
    }
#endif
}