using UnityEngine;

public class EnterObjectManager : MonoBehaviour {
    [field: Header("Settings")]
    [field : SerializeField] public Vector3 CameraTargetPos { get; private set; } = Vector3.zero;
}