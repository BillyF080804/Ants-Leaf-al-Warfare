using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrownScript : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private float cameraDelay = 10.0f;
    [SerializeField] private Vector3 cameraOverviewPos = new Vector3(-85, 10, -15);
    [SerializeField] private GameObject rippleParticle;
    [SerializeField] private List<PositionInfo> positionInfo = new List<PositionInfo>();

    [Serializable]
    private class PositionInfo {
        public Vector3 position;
        public float duration;
    }

    private CameraSystem cameraSystem;
    private TurnManager turnManager;

    private void Start() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<Ant>(out Ant ant)) {
            cameraSystem.SetCameraLookAtTarget(other.transform);
            cameraSystem.SetCameraTarget(cameraOverviewPos, true);
            cameraSystem.CameraDelay(cameraDelay);

            ant.SetCanMove(false);
            turnManager.CurrentAntTurn.SetCanMove(false);
            Instantiate(rippleParticle, other.transform.position, Quaternion.identity);
            StartCoroutine(DrownCoroutine(other.transform, ant));
        }
    }

    private IEnumerator DrownCoroutine(Transform antTransform, Ant antScript) {
        Vector3 targetOne = antTransform.position;
        targetOne.y -= 2.5f;
        StartCoroutine(Lerp(antTransform.position, targetOne, 0.5f, antTransform));
        yield return new WaitUntil(() => antTransform.position == targetOne);

        foreach (PositionInfo posInfo in positionInfo) {
            StartCoroutine(Lerp(antTransform.position, posInfo.position, posInfo.duration, antTransform));
            yield return new WaitUntil(() => antTransform.position == posInfo.position);
        }

        antScript.TakeDamage(1000);
        cameraSystem.SetCameraLookAtTarget(null);
        cameraSystem.SetCameraTarget(null);
        yield return new WaitForSeconds(2.5f);
        turnManager.EndTurn();
    }

    private IEnumerator Lerp(Vector3 startPos, Vector3 endPos, float duration, Transform ant) {
        float timeElapsed = 0.0f;

        while (timeElapsed < duration) {
            ant.transform.position = LerpLibrary.ObjectLerp(startPos, endPos, LerpType.InOut, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        ant.transform.position = endPos;
    }
}