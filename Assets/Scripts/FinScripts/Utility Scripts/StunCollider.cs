using UnityEngine;

public class StunCollider : MonoBehaviour {
    [SerializeField] private FlyTrapHazard trapHazard;


    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<WeaponScript>(out WeaponScript weaponScript)) {
            if (weaponScript.weaponInfo.cameraShakeOnImpact) {
                FindFirstObjectByType<CameraSystem>().StartCameraShake(weaponScript.weaponInfo.cameraShakeDuration, weaponScript.weaponInfo.cameraShakeIntensity);
            }

            if (weaponScript.weaponInfo.hasVFX == true && weaponScript.weaponInfo.vfxObject != null) {
                GameObject vfxObj = Instantiate(weaponScript.weaponInfo.vfxObject, transform.position, Quaternion.identity);
                vfxObj.transform.localScale = new Vector3(weaponScript.weaponInfo.vfxSize, weaponScript.weaponInfo.vfxSize, weaponScript.weaponInfo.vfxSize);
                Destroy(vfxObj, weaponScript.weaponInfo.vfxDuration);
            }

            if (weaponScript.weaponInfo.hasSounds && weaponScript.weaponInfo.impactSound != null) {
                FindFirstObjectByType<WeaponManager>().AudioPlayer.ChangeClip(weaponScript.weaponInfo.impactSound);
                FindFirstObjectByType<WeaponManager>().AudioPlayer.PlayClip();
            }

            trapHazard.GetHit();
            Destroy(weaponScript.gameObject);
        }
    }
}
