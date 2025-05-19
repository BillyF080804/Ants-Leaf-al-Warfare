using System.Collections.Generic;
using UnityEngine;

public class EnterObjectManager : MonoBehaviour {
    [field: Header("Settings")]
    [field: SerializeField] public Vector3 CameraTargetPos { get; private set; } = Vector3.zero;

    private Transform triggerLeft;
    private Transform triggerRight;
    private List<Ant> ants = new List<Ant>();
    private TurnManager turnManager;
    private CameraSystem cameraSystem;

    private void Awake() {
        turnManager = FindFirstObjectByType<TurnManager>();
        cameraSystem = FindFirstObjectByType<CameraSystem>();
    }

    public void SetTrigger(bool isLeftTrigger, Transform newTransform) {
        if (isLeftTrigger) {
            triggerLeft = newTransform;
        }
        else {
            triggerRight = newTransform;
        }
    }

    public void OnAllAntsSpawned() {
        foreach (Player player in turnManager.PlayerList) {
            foreach (GameObject ant in player.AntList) {
                if (ant.transform.position.x > triggerLeft.position.x && ant.transform.position.x < triggerRight.position.x) {
                    ants.Add(ant.GetComponent<Ant>());
                }
            }
        }
    }

    public void StartTurnEvent() {
        if (ants.Contains(turnManager.CurrentAntTurn)) {
            cameraSystem.SetCameraLookAtTarget(turnManager.CurrentAntTurn.transform);
            cameraSystem.SetCameraTarget(CameraTargetPos, 0, 0);
        }
    }

    public void AddAnt(Ant ant) {
        if (!ants.Contains(ant)) {
            ants.Add(ant);
        }
    }

    public void RemoveAnt(Ant ant) {
        if (ants.Contains(ant)) {
            ants.Remove(ant);
        }
    }
}