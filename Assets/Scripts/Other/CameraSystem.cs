using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraSystem : MonoBehaviour {
    [Header("Camera Follow Settings")]
    [SerializeField] private float yOffset = 5f;
    [SerializeField] private float zOffset = -15.0f;
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

    [Header("Camera Pan Settings")]
    [SerializeField] private MoveUI topBlackBar;
    [SerializeField] private MoveUI bottomBlackBar;
    [SerializeField] private float panDuration = 10.0f;
    [SerializeField] private List<Vector3> panPositions = new List<Vector3>();

    private bool freeCamEnabled = false;
    private bool cameraZoomingEnabled = true;
    private bool smoothingEnabled = false;
    private float cameraZoom = 0.0f;
    private float cameraZoomValue = 0.0f;

    private Vector2 freeCamValue = Vector2.zero;
    private Vector3 freeCamPos = Vector3.zero;
    private Vector3 targetPos = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 lookAtTargetPos = Vector3.zero;
    private Vector3 overviewPosition = Vector3.zero;

    private Transform lookAtTarget = null;
    private GameObject cameraObj;
    private Camera cameraComp;
    private Coroutine cameraZoomCoroutine;
    private List<Transform> targets = new List<Transform>();

    private WeaponManager weaponManager;
    private VolumeProfile volumeProfile;

    public delegate void OnIterationFinished(Transform target);
    public static OnIterationFinished onIterationFinished;

    public bool IsFOVZoomingOut { get; private set; } = false;
    public bool CameraDelayActive { get; set; } = false;
    public Transform CameraTarget { get; private set; }

    private void Awake() {
        cameraObj = gameObject;
        cameraComp = GetComponentInChildren<Camera>();
        overviewPosition = cameraObj.transform.position;

        weaponManager = FindFirstObjectByType<WeaponManager>();
        volumeProfile = FindFirstObjectByType<Volume>().profile;
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
        else if (smoothingEnabled == true) {
            SetCameraPos();
        }

        CheckLookAtTarget();
    }

    private void SetCameraZoom() {
        if (cameraZoomingEnabled == true) {
            cameraZoom += cameraZoomValue * cameraZoomSpeed;
            cameraZoom = Mathf.Clamp(cameraZoom, maxZoomOut, maxZoomIn);
        }
    }

    public void SetCameraZoomingBool(bool isEnabled) {
        cameraZoomingEnabled = isEnabled;
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

        if (weaponManager.WeaponSelected != null) {
            zOffset = -30.0f;
            yOffset = 10.0f;
        }
        else {
            zOffset = -15.0f;
            yOffset = 5.0f;
        }

        if (CameraTarget == null && targetPos != Vector3.zero) {
            ChangeBlur(60);
            targetPos.z += cameraZoom;
            cameraObj.transform.position = Vector3.SmoothDamp(cameraObj.transform.position, targetPos, ref velocity, smoothTime);
        }
        else {
            if (CameraTarget != null && targetPos == Vector3.zero) {
                ChangeBlur(60);
                tempTargetPos = new Vector3(CameraTarget.position.x, CameraTarget.position.y + yOffset, zOffset);
            }
            else if (CameraTarget == null && targetPos == Vector3.zero) {
                ChangeBlur(175);
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
        float startingVignette = 0.0f;
        Vignette vignette = null;
        IsFOVZoomingOut = !zoomIn;

        if (volumeProfile.TryGet(out Vignette newVignette)) {
            startingVignette = newVignette.intensity.value;
            vignette = newVignette;
        }

        while (timeElapsed < zoomDuration) {
            if (zoomIn == true) {
                cameraComp.fieldOfView = Mathf.Lerp(startingZoom, 30, timeElapsed / zoomDuration);

                if (vignette != null) {
                    vignette.intensity.Override(Mathf.Lerp(startingVignette, 1.0f, timeElapsed / zoomDuration));
                }
            }
            else {
                cameraComp.fieldOfView = Mathf.Lerp(startingZoom, 60, timeElapsed / zoomDuration);

                if (vignette != null) {
                    vignette.intensity.Override(Mathf.Lerp(startingVignette, 0.1f, timeElapsed / zoomDuration));
                }
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
        if (!targets.Contains(newTarget)) {
            targets.Add(newTarget);
        }
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
            yield return new WaitForSeconds(0.75f);
            onIterationFinished?.Invoke(target);
            yield return new WaitForSeconds(delay);
        }

        targets.Clear();
        CameraDelayActive = false;
    }

    public void StartCameraPan() {
        StartCoroutine(CameraPanCoroutine());
    }

    private IEnumerator CameraPanCoroutine() {
        CameraDelayActive = true;

        for (int i = 0; i < panPositions.Count - 1; i++) {
            StartCoroutine(Lerp(panPositions[i], panPositions[i + 1], panDuration));
            yield return new WaitUntil(() => cameraObj.transform.position == panPositions[i + 1]);
        }

        topBlackBar.StartMoveUI(LerpType.InOut, Vector2.zero, new Vector2(0, 100), 1.0f, true);
        bottomBlackBar.StartMoveUI(LerpType.InOut, Vector2.zero, new Vector2(0, -100), 1.0f, true);
        smoothingEnabled = true;
        CameraDelayActive = false;
    }

    private IEnumerator Lerp(Vector3 startPos, Vector3 endPos, float duration) {
        float timeElapsed = 0.0f;

        while (timeElapsed < duration) {
            cameraObj.transform.position = LerpLibrary.ObjectLerp(startPos, endPos, LerpType.InOut, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        cameraObj.transform.position = endPos;
    }

    public void ResetCameraToFollowAnt() {
        ResetCamera();
        SetCameraTarget(FindFirstObjectByType<TurnManager>().CurrentAntTurn.transform);
        SetCameraLookAtTarget(null);
    }

    private void ChangeBlur(float value) {
        if (volumeProfile.TryGet(out DepthOfField depthOfField)) {
            depthOfField.gaussianStart.Override(value);
        }
    }
}