using System.Collections;
using UnityEditor;
using UnityEngine;

public class CameraSystem : MonoBehaviour {
    [Header("Camera Follow Settings")]
    [SerializeField] private float yOffset = 2.5f;
    [SerializeField] private float zOffset = -10.0f;
    [SerializeField] private float smoothTime = 0.3f;

    [Header("Misc Settings")]
    public bool setManualOverviewPosition;

    private Vector3 velocity = Vector3.zero;
    private Transform cameraTarget;
    private Camera cameraComp;
    private Coroutine cameraZoomCoroutine;

    public bool IsZoomingOut { get; private set; } = false;

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

    private void Update() {
        if (cameraComp != null) { 
            if (cameraTarget != null) {
                Vector3 targetPos = new Vector3(cameraTarget.position.x, cameraTarget.position.y + yOffset, zOffset);

                cameraComp.transform.position = Vector3.SmoothDamp(cameraComp.transform.position, targetPos, ref velocity, smoothTime);
            }
        }
    }

    public void SetCameraTarget(Transform target) {
        cameraTarget = target;
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