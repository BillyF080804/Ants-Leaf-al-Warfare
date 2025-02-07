using System.Collections;
using UnityEngine;

public class CameraSystem : MonoBehaviour {
    private Camera cameraComp;
    private Coroutine cameraZoomCoroutine;

    public bool IsZoomingOut { get; private set; } = false;

    private void Awake() {
        cameraComp = GetComponent<Camera>();
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