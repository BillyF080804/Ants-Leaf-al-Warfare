using UnityEngine;

public class BridgeCollider : MonoBehaviour {
    private DestructableBridge bridge;
    private CameraSystem cameraSystem;

    private void Start() {
        bridge = transform.parent.GetComponent<DestructableBridge>();
        cameraSystem = FindFirstObjectByType<CameraSystem>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.TryGetComponent(out WeaponScript weaponScript) && bridge.HasCollapsed == false) {
            bridge.TakeDamage(weaponScript.weaponInfo.baseDamage);

            if (weaponScript.weaponInfo.hasVFX == true && weaponScript.weaponInfo.vfxObject != null) {
                if (weaponScript.weaponInfo.explosive && weaponScript.weaponInfo.explodeOnImpact) {
                    weaponScript.CreateVFX();
                }
                else if (weaponScript.weaponInfo.explosive == false) {
                    weaponScript.CreateVFX();
                }
            }

            cameraSystem.StartCameraShake(0.5f, 1.0f);
        }
    }
}