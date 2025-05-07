using UnityEngine;

public class SprayAreaScript : MonoBehaviour {
    private BaseWeaponSO weaponInfo;
    private TurnManager turnManager;

    public void Setup(BaseWeaponSO _weaponInfo, TurnManager _turnManager) {
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
        if (other.gameObject != turnManager.CurrentAntTurn.gameObject && other.TryGetComponent(out Ant ant)) {
            Debug.Log("inside");
            ant.TakeDamage(weaponInfo.baseDamage);

            if (weaponInfo.sprayMover == true) {
                ant.UnFreezeMovement();
                ant.GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.sprayStrength, turnManager.CurrentAntTurn.transform.position, weaponInfo.sprayLength, 2f, ForceMode.Impulse);
            }

            if (weaponInfo.weaponEffect != null) {
                weaponInfo.weaponEffect.GetComponent<EffectScript>().AddEffect(ant);
            }
        }
    }
}