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

    private void Awake() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        turnManager = FindFirstObjectByType<TurnManager>();

        if (triggerType == TriggerType.Left) {
            manager.SetTrigger(true, transform);
        }
        else {
            manager.SetTrigger(false, transform);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<Ant>(out Ant ant) && turnManager.CurrentAntTurn == ant) {
            AddAnt(ant);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent<Ant>(out Ant ant) && turnManager.CurrentAntTurn == ant) {
            if (triggerType == TriggerType.Left && ant.transform.position.x < transform.position.x) {
                RemoveAnt(ant);
            }
            else if (triggerType == TriggerType.Left && ant.transform.position.x > transform.position.x) {
                AddAnt(ant);
            }
            else if (triggerType == TriggerType.Right && ant.transform.position.x > transform.position.x) {
                RemoveAnt(ant);
            }
            else if (triggerType == TriggerType.Right && ant.transform.position.x < transform.position.x) {
                AddAnt(ant);
            }
        }
    }

    private void AddAnt(Ant ant) {
        manager.AddAnt(ant);
        cameraSystem.SetCameraLookAtTarget(ant.transform);
        cameraSystem.SetCameraTarget(manager.CameraTargetPos, 0, 0);
    }

    private void RemoveAnt(Ant ant) {
        manager.RemoveAnt(ant);
        cameraSystem.ResetCameraToFollowAnt();
    }
}