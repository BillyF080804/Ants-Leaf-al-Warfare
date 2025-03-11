using System.Linq;
using UnityEngine;

public class QueenAntSpawner : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private float queenSpawningSpeed = 10.0f;

    public bool QueenInValidPos { get; private set; } = false;
    private float moveValue = 0.0f;
    private GameObject targetObject;
    private TurnManager turnManager;

    private void Start() {
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    public void SetMoveValue(float value, GameObject _targetObject) {
        moveValue = value;
        targetObject = _targetObject;
    }

    public void FinishSpawning() {
        gameObject.SetActive(false);
    }

    private void Update() {
        if (targetObject != null) {
            float xPos = targetObject.transform.position.x + moveValue * queenSpawningSpeed * Time.deltaTime;
            xPos = Mathf.Clamp(xPos, turnManager.MapMinX, turnManager.MapMaxX);
            Vector3 targetPos = new Vector3(xPos, 30.0f, 0);

            if (Physics.Raycast(targetPos, Vector3.down, out RaycastHit ray, 35.0f, ~targetObject.GetComponent<QueenAntScript>().GetQueenLayerMask())) {
                targetObject.transform.position = new Vector3(ray.point.x, ray.point.y + 0.5f, 0);
            }

            CheckQueenInValidPos();
        }
    }

    public void CheckQueenInValidPos() {
        if (Physics.OverlapSphere(targetObject.transform.position, turnManager.MinDistanceBetweenQueens).Where(x => x.CompareTag("Player")).Count() > 0) {
            QueenInValidPos = false;
            targetObject.GetComponent<QueenAntScript>().SetQueenInvalidPos();
        }
        else {
            QueenInValidPos = true;
            targetObject.GetComponent<QueenAntScript>().SetQueenValidPos();
        }
    }
}