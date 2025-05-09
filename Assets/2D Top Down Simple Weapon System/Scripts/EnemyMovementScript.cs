using UnityEngine;
using WeaponSystem;

public class EnemyMovementScript : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 2.5f;

    private Transform player;

    private void Start() {
        player = FindFirstObjectByType<PlayerMovementScript>().transform;
    }

    private void Update() {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }
}