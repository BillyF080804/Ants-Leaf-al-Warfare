using System.Collections;
using UnityEngine;

public class DrownScript : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private float cameraDelay = 3.0f;
    [SerializeField] private Vector3 cameraOverviewPos = new Vector3(-85, 10, -15);
    [SerializeField] private GameObject rippleParticle;

    private CameraSystem cameraSystem;
    private TurnManager turnManager;

    private void Start() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        turnManager = FindFirstObjectByType<TurnManager>();
    }
    
    //Called when ants enter water
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<Ant>(out Ant ant)) {
            cameraSystem.SetCameraLookAtTarget(other.transform);
            cameraSystem.SetCameraTarget(cameraOverviewPos, true);
            cameraSystem.CameraDelay(cameraDelay);
            GetComponent<AudioPlayer>().PlayClip();

            ant.isDrowning = true;
            ant.SetCanMove(false);
            turnManager.CurrentAntTurn.SetCanMove(false);
            Instantiate(rippleParticle, other.transform.position, Quaternion.identity);
            StartCoroutine(DrownCoroutine(other.transform, ant));
        }
    }

    //Bob ant up and down then kill ant
    private IEnumerator DrownCoroutine(Transform antTransform, Ant antScript) {
        Vector3 drownPos = antTransform.position;

        drownPos.y -= 2.5f;
        StartCoroutine(Lerp(antTransform.position, drownPos, 0.5f, antTransform));
        yield return new WaitUntil(() => antTransform.position == drownPos);

        drownPos.y += 2.5f;
        StartCoroutine(Lerp(antTransform.position, drownPos, 0.5f, antTransform));
        yield return new WaitUntil(() => antTransform.position == drownPos);

        drownPos.y -= 2.5f;
        StartCoroutine(Lerp(antTransform.position, drownPos, 2.5f, antTransform));
        yield return new WaitUntil(() => antTransform.position == drownPos);

        cameraSystem.SetCameraLookAtTarget(null);
        cameraSystem.SetCameraTarget(null);
        antScript.TakeDamage(1000);
        StartCoroutine(turnManager.EndTurnCoroutine());
    }

    private IEnumerator Lerp(Vector3 startPos, Vector3 endPos, float duration, Transform ant) {
        float timeElapsed = 0.0f;

        while (timeElapsed < duration) {
            ant.transform.position = LerpLibrary.ObjectLerp(startPos, endPos, LerpType.InOut, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (ant != null) {
            ant.transform.position = endPos;
        }
    }
}