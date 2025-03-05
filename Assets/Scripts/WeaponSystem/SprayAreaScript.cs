using UnityEngine;

public class SprayAreaScript : MonoBehaviour {
    private Collider playerCollider;
    private BaseWeaponSO weaponInfo;
    private TurnManager turnManager;

    public void Setup(Collider _playerCollider, BaseWeaponSO _weaponInfo, TurnManager _turnManager) {
        playerCollider = _playerCollider;
        weaponInfo = _weaponInfo;
        turnManager = _turnManager;

        if (weaponInfo.sprayAreaVFX != null) {
            GameObject vfx = Instantiate(weaponInfo.sprayAreaVFX, transform.position, Quaternion.identity, transform);
            vfx.transform.localRotation = Quaternion.Euler(Vector3.zero);
            vfx.transform.localScale = new Vector3(weaponInfo.sprayAreaVFXSize, weaponInfo.sprayAreaVFXSize, weaponInfo.sprayAreaVFXSize);
        }

        Destroy(gameObject, weaponInfo.sprayDuration);
    }

    private void OnTriggerEnter(Collider other) {
        if (other != playerCollider && other.TryGetComponent(out Ant ant)) {
            ant.TakeDamage(weaponInfo.baseDamage);

            if (weaponInfo.sprayMover == true) {
                ant.GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.sprayStrength, turnManager.CurrentAntTurn.transform.position, weaponInfo.sprayLength, 2f);
            }

            if (weaponInfo.weaponEffect != null) {
                weaponInfo.weaponEffect.AddEffect(ant);
            }
        }
    }
}