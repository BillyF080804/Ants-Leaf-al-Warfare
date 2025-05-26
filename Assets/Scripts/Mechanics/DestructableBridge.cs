using UnityEngine;

public class DestructableBridge : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private float health;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Collider To Disable")]
    [SerializeField] private GameObject colToDisable;

    public bool HasCollapsed { get; private set; } = false;
    private CameraSystem cameraSystem;

    private void Start() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
    }

    public void TakeDamage(int damageToDeal) {
        health -= damageToDeal;

        if (health <= 0) {
            CollapseBridge();
        }
    }

    //Called when the bridge collapses
    private void CollapseBridge() {
        HasCollapsed = true;
        colToDisable.SetActive(false);
        cameraSystem.StartCameraShake(2.0f, 2.0f);
        cameraSystem.SetCameraTarget(transform.position, 10.0f, 30.0f);
        cameraSystem.CameraDelay(5.0f);

        GetComponent<AudioPlayer>().PlayClip();
        animator.SetTrigger("OnDestruct");
    }
}