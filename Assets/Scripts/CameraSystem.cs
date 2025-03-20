using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraSystem : MonoBehaviour {
    [Header("Camera Follow Settings")]
    [SerializeField] private float yOffset = 2.5f;
    [SerializeField] private float zOffset = -10.0f;
    [SerializeField] private float smoothTime = 0.3f;

    [Header("Zoom Settings")]
    [SerializeField] private float maxZoomIn = 5f;
    [SerializeField] private float maxZoomOut = -10f;
    [SerializeField] private float cameraZoomSpeed = 0.025f;

    [Header("Free Cam Settings")]
    [SerializeField] private float freeCamSpeed = 0.1f;
    [SerializeField] private float freeCamMinX = -20.0f;
    [SerializeField] private float freeCamMaxX = 20.0f;
    [SerializeField] private float freeCamMinY = 0.0f;
    [SerializeField] private float freeCamMaxY = 20.0f;

    [Header("Misc Settings")]
    public bool setManualOverviewPosition;

    private bool freeCamEnabled = false;
    private float cameraZoom = 0.0f;
    private float cameraZoomValue = 0.0f;

    private Vector2 freeCamValue = Vector2.zero;
    private Vector3 freeCamPos = Vector3.zero;
    private Vector3 targetPos = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 lookAtTargetPos = Vector3.zero;

    private Transform lookAtTarget = null;
    private GameObject cameraObj;
    private Camera cameraComp;
    private Coroutine cameraZoomCoroutine;
    private List<Transform> targets = new List<Transform>();

    public delegate void OnIterationFinished(Transform target);
    public static OnIterationFinished onIterationFinished;

    public bool IsFOVZoomingOut { get; private set; } = false;
    public bool CameraDelayActive { get; private set; } = false;
    public Transform CameraTarget { get; private set; }

    [HideInInspector] public Vector3 overviewPosition;

    private void Awake() {
        cameraObj = gameObject;
        cameraComp = GetComponentInChildren<Camera>();

        if (!setManualOverviewPosition) {
            overviewPosition = cameraObj.transform.position;
        }
        else {
            cameraObj.transform.position = overviewPosition;
        }
    }

    private void LateUpdate() {
        SetCameraZoom();

        if (freeCamValue != Vector2.zero && freeCamEnabled == false) {
            freeCamEnabled = true;
            freeCamPos = cameraObj.transform.position;
        }
        else if (freeCamEnabled == true) {
            SetFreeCam();
        }
        else {
            SetCameraPos();
        }

        CheckLookAtTarget();
    }

    private void SetCameraZoom() {
        cameraZoom += cameraZoomValue * cameraZoomSpeed;
        cameraZoom = Mathf.Clamp(cameraZoom, maxZoomOut, maxZoomIn);
    }

    private void SetFreeCam() {
        freeCamPos += (Vector3)freeCamValue * freeCamSpeed;

        float clampedX = Mathf.Clamp(freeCamPos.x, freeCamMinX, freeCamMaxX);
        float clampedY = Mathf.Clamp(freeCamPos.y, freeCamMinY, freeCamMaxY);

        freeCamPos.x = clampedX;
        freeCamPos.y = clampedY;
        freeCamPos.z = -10.0f;
        freeCamPos.z += cameraZoom;

        cameraObj.transform.position = Vector3.SmoothDamp(cameraObj.transform.position, freeCamPos, ref velocity, smoothTime);
    }

    private void SetCameraPos() {
        Vector3 tempTargetPos = Vector3.zero;

        if (CameraTarget == null && targetPos != Vector3.zero) {
            targetPos.z += cameraZoom;
            cameraObj.transform.position = Vector3.SmoothDamp(cameraObj.transform.position, targetPos, ref velocity, smoothTime);
        }
        else {
            if (CameraTarget != null && targetPos == Vector3.zero) {
                tempTargetPos = new Vector3(CameraTarget.position.x, CameraTarget.position.y + yOffset, zOffset);
            }
            else if (CameraTarget == null && targetPos == Vector3.zero) {
                tempTargetPos = new Vector3(overviewPosition.x, overviewPosition.y + yOffset, overviewPosition.z);
            }

            tempTargetPos.z += cameraZoom;
            cameraObj.transform.position = Vector3.SmoothDamp(cameraObj.transform.position, tempTargetPos, ref velocity, smoothTime);
        }
    }

    private void CheckLookAtTarget() {
        if (lookAtTarget != null) {
            cameraObj.transform.LookAt(lookAtTarget);
        }
        else if (lookAtTargetPos != Vector3.zero) {
            cameraObj.transform.LookAt(lookAtTargetPos);
        }
        else {
            cameraObj.transform.eulerAngles = new Vector3(15, 0, 0);
        }
    }

    public void SetCameraTarget(Transform target) {
        targetPos = Vector3.zero;
        CameraTarget = target;
    }

    public void SetCameraTarget(Transform target, bool resetCameraZoom) {
        targetPos = Vector3.zero;
        CameraTarget = target;

        if (resetCameraZoom) {
            cameraZoomValue = 0.0f;
            cameraZoom = 0.0f;
        }
    }

    public void SetCameraTarget(Vector3 _targetPos) {
        CameraTarget = null;
        targetPos = new Vector3(_targetPos.x, _targetPos.y + 10f, _targetPos.z - 25f);
    }

    public void SetCameraTarget(Vector3 _targetPos, bool resetCameraZoom) {
        CameraTarget = null;
        targetPos = new Vector3(_targetPos.x, _targetPos.y + 10f, _targetPos.z - 25f);

        if (resetCameraZoom) {
            cameraZoomValue = 0.0f;
            cameraZoom = 0.0f;
        }
    }

    public void SetCameraTarget(Vector3 _targetPos, float yOffset, float zOffset) {
        CameraTarget = null;
        targetPos = new Vector3(_targetPos.x, _targetPos.y + yOffset, _targetPos.z - zOffset);
    }

    public void SetCameraTarget(Vector3 _targetPos, float yOffset, float zOffset, bool resetCameraZoom) {
        CameraTarget = null;
        targetPos = new Vector3(_targetPos.x, _targetPos.y + yOffset, _targetPos.z - zOffset);

        if (resetCameraZoom) {
            cameraZoomValue = 0.0f;
            cameraZoom = 0.0f;
        }
    }

    public void SetCameraLookAtTarget(Transform target) {
        lookAtTarget = target;
        lookAtTargetPos = Vector3.zero;
    }

    public void SetCameraLookAtTarget(Vector3 targetVector) {
        lookAtTarget = null;
        lookAtTargetPos = targetVector;
    }

    public void CameraDelay(float delayTime) {
        StartCoroutine(CameraDelayCoroutine(delayTime));
    }

    private IEnumerator CameraDelayCoroutine(float delayTime) {
        CameraDelayActive = true;
        yield return new WaitForSeconds(delayTime);
        CameraDelayActive = false;
    }

    public void ZoomCameraFOVIn(float zoomDuration) {
        if (cameraZoomCoroutine != null) {
            StopCoroutine(cameraZoomCoroutine);
        }

        cameraZoomCoroutine = StartCoroutine(CameraFOVZoom(true, zoomDuration));
    }

    public void ZoomCameraFOVOut(float zoomDuration) {
        if (cameraZoomCoroutine != null) {
            StopCoroutine(cameraZoomCoroutine);
        }

        cameraZoomCoroutine = StartCoroutine(CameraFOVZoom(false, zoomDuration));
    }

    private IEnumerator CameraFOVZoom(bool zoomIn, float zoomDuration) {
        float timeElapsed = 0.0f;
        float startingZoom = cameraComp.fieldOfView;
        IsFOVZoomingOut = !zoomIn;

        while (timeElapsed < zoomDuration) {
            if (zoomIn == true) {
                cameraComp.fieldOfView = Mathf.Lerp(startingZoom, 30, timeElapsed / zoomDuration);
            }
            else {
                cameraComp.fieldOfView = Mathf.Lerp(startingZoom, 60, timeElapsed / zoomDuration);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        IsFOVZoomingOut = false;

        if (zoomIn == true) {
            cameraComp.fieldOfView = 30;
        }
        else {
            cameraComp.fieldOfView = 60;
        }
    }

    public void StartCameraShake(float shakeDuration, float shakeIntensity) {
        StartCoroutine(CameraShakeCoroutine(shakeDuration, shakeIntensity));
    }

    private IEnumerator CameraShakeCoroutine(float duration, float strength) {
        float timeElapsed = 0.0f;
        Vector3 originalEulerAngles = cameraComp.transform.localEulerAngles;

        while (timeElapsed < duration) {
            float randomX = Random.value - 0.5f * strength;
            float randomY = Random.value - 0.5f * strength;
            float randomZ = Random.value - 0.5f * strength;

            cameraComp.transform.localEulerAngles = new Vector3(originalEulerAngles.x + randomX, originalEulerAngles.y + randomY, originalEulerAngles.z + randomZ);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void SetCameraZoom(float _cameraZoom) {
        cameraZoomValue = _cameraZoom;
    }

    public void SetFreeCameraValue(Vector2 value) {
        freeCamValue = value;
    }

    public void ResetCamera() {
        cameraZoomValue = 0.0f;
        cameraZoom = 0.0f;

        freeCamEnabled = false;
        freeCamValue = Vector2.zero;
        freeCamPos = Vector3.zero;
    }

    public void AddNewCameraTarget(Transform newTarget) {
        targets.Add(newTarget);
    }

    public void IterateCameraTargets(float holdDelay) {
        if (targets.Count > 0) {
            StartCoroutine(IterateTargetsCoroutine(holdDelay));
        }
    }

    private IEnumerator IterateTargetsCoroutine(float delay) {
        CameraDelayActive = true;

        foreach (Transform target in targets) {
            SetCameraTarget(target.position, 2.0f, 7.5f);
            yield return new WaitForSeconds(0.5f);
            onIterationFinished?.Invoke(target);
            yield return new WaitForSeconds(delay);
        }

        CameraDelayActive = false;
    }
}

#if (UNITY_EDITOR)
[CustomEditor(typeof(CameraSystem))]
public class CameraSystemEditor : Editor {
    [InitializeOnEnterPlayMode]
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        CameraSystem cameraSystem = (CameraSystem)target;

        if (cameraSystem.setManualOverviewPosition) {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Overview Settings", EditorStyles.boldLabel);
            cameraSystem.overviewPosition = EditorGUILayout.Vector3Field("Overview Position", cameraSystem.overviewPosition);
        }
    }
}
#endif