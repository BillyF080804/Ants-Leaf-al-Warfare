using UnityEngine;

public class SprayAreaScript : MonoBehaviour {
    private Collider playerCollider;
    private BaseWeaponSO weaponInfo;

    public void Setup(Collider _playerCollider, BaseWeaponSO _weaponInfo) {
        playerCollider = _playerCollider;
        weaponInfo = _weaponInfo;

        if (weaponInfo.sprayAreaVFX != null) {
            GameObject vfx = Instantiate(weaponInfo.sprayAreaVFX, transform.position, Quaternion.identity, transform);
            vfx.transform.localRotation = Quaternion.Euler(Vector3.zero);
            vfx.transform.localScale = new Vector3(weaponInfo.sprayAreaVFXSize, weaponInfo.sprayAreaVFXSize, weaponInfo.sprayAreaVFXSize);
        }

        Destroy(gameObject, weaponInfo.sprayDuration);
    }

    private void OnTriggerEnter(Collider other) {
        if (other != playerCollider) {
            other.GetComponent<Ant>().TakeDamage(weaponInfo.baseDamage);
            Debug.Log(other.name);
        }
    }
}