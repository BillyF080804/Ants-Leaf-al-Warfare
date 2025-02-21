using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraSystem : MonoBehaviour {
    [Header("Camera Follow Settings")]
    [SerializeField] private float yOffset = 2.5f;
    [SerializeField] private float zOffset = -10.0f;
    [SerializeField] private float smoothTime = 0.3f;

    [Header("Misc Settings")]
    public bool setManualOverviewPosition;

    private Vector3 targetPos = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Camera cameraComp;
    private Coroutine cameraZoomCoroutine;

    public bool IsZoomingOut { get; private set; } = false;
    public bool CameraDelayActive { get; private set; } = false;
    public Transform CameraTarget { get; private set; }

    [HideInInspector] public Vector3 overviewPosition;

    private void Awake() {
        cameraComp = GetComponent<Camera>();

        if (!setManualOverviewPosition) {
            overviewPosition = cameraComp.transform.position;
        }
        else {
            cameraComp.transform.position = overviewPosition;
        }
    }

    private void LateUpdate() {
        Vector3 tempTargetPos = Vector3.zero;

        if (CameraTarget == null && targetPos != Vector3.zero) {
            cameraComp.transform.position = Vector3.SmoothDamp(cameraComp.transform.position, targetPos, ref velocity, smoothTime);
        }
        else {
            if (CameraTarget != null && targetPos == Vector3.zero) {
                tempTargetPos = new Vector3(CameraTarget.position.x, CameraTarget.position.y + yOffset, zOffset);
            }
            else if (CameraTarget == null && targetPos == Vector3.zero) {
                tempTargetPos = new Vector3(overviewPosition.x, overviewPosition.y + yOffset, overviewPosition.z);
            }

            cameraComp.transform.position = Vector3.SmoothDamp(cameraComp.transform.position, tempTargetPos, ref velocity, smoothTime);
        }
    }

    public void SetCameraTarget(Transform target) {
        targetPos = Vector3.zero;
        CameraTarget = target;
    }

    public void SetCameraTarget(Vector3 _targetPos) {
        CameraTarget = null;
        targetPos = new Vector3(_targetPos.x, _targetPos.y + 10f, _targetPos.z - 25f);
    }

    public void SetCameraTarget(Vector3 _targetPos, float yOffset, float zOffset) {
        CameraTarget = null;
        targetPos = new Vector3(_targetPos.x, _targetPos.y + yOffset, _targetPos.z - zOffset);
    }

    public void CameraDelay(float delayTime) {
        StartCoroutine(CameraDelayCoroutine(delayTime));
    }

    private IEnumerator CameraDelayCoroutine(float delayTime) {
        CameraDelayActive = true;
        yield return new WaitForSeconds(delayTime);
        CameraDelayActive = false;
    }

    public void ZoomCameraIn(float zoomDuration) {
        if (cameraZoomCoroutine != null) {
            StopCoroutine(cameraZoomCoroutine);
        }

        cameraZoomCoroutine = StartCoroutine(CameraZoom(true, zoomDuration));
    }

    public void ZoomCameraOut(float zoomDuration) {
        if (cameraZoomCoroutine != null) {
            StopCoroutine(cameraZoomCoroutine);
        }

        cameraZoomCoroutine = StartCoroutine(CameraZoom(false, zoomDuration));
    }

    private IEnumerator CameraZoom(bool zoomIn, float zoomDuration) {
        float timeElapsed = 0.0f;
        float startingZoom = cameraComp.fieldOfView;
        IsZoomingOut = !zoomIn;

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

        IsZoomingOut = false;

        if (zoomIn == true) {
            cameraComp.fieldOfView = 30;
        }
        else {
            cameraComp.fieldOfView = 60;
        }
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