using UnityEngine;

public class TriggerScript : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private EnterObjectManager manager;
    [SerializeField] private TriggerType triggerType;
    private CameraSystem cameraSystem;
    private TurnManager turnManager;

    private enum TriggerType {
        Left,
        Right 
    };

    private void Start() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<Ant>(out Ant ant) && turnManager.CurrentAntTurn == ant) {
            cameraSystem.SetCameraLookAtTarget(ant.transform);
            cameraSystem.SetCameraTarget(manager.CameraTargetPos, 0, 0);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent<Ant>(out Ant ant) && turnManager.CurrentAntTurn == ant) {
            if (triggerType == TriggerType.Left && ant.transform.position.x < transform.position.x) {
                ResetCamera();
            }
            else if (triggerType == TriggerType.Right && ant.transform.position.x > transform.position.x) {
                ResetCamera();
            }
        }
    }

    private void ResetCamera() {
        cameraSystem.ResetCameraToFollowAnt();
    }
}