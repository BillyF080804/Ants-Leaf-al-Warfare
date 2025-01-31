using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponScript : MonoBehaviour {
    private BaseWeaponSO weaponInfo;

    public void SetupWeapon(BaseWeaponSO weapon) {
        weaponInfo = weapon;
        Destroy(gameObject, 10.0f);
    }

    public void StartFuse() {
        StartCoroutine(FuseTimer());
    }

    private IEnumerator FuseTimer() {
        float fuseTimer = weaponInfo.fuseTimer;

        while (fuseTimer > 0) {
            fuseTimer -= Time.deltaTime;
            yield return null;
        }

        Explode();
    }

    private void Explode() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, weaponInfo.explosionRange).Where(x => x.CompareTag("Player")).ToArray();

        foreach (Collider collider in colliders) {
            Debug.Log(collider.name + " damage dealt: " + weaponInfo.baseDamage);
            CheckAntType(collider.gameObject);
            collider.GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.explosionPower, transform.position, weaponInfo.explosionRange, 3, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (weaponInfo.explosive && weaponInfo.explodeOnImpact) {
            Explode();
        }
        else if (collision.gameObject.CompareTag("Player")) {
            CheckAntType(collision.gameObject);
        }

        Destroy(gameObject);
    }

    private void CheckAntType(GameObject gameObjectToCheck) {
        if (gameObjectToCheck.TryGetComponent(out AntScript antScript)) {
            antScript.TakeDamage(weaponInfo.baseDamage);
        }
        else {
            gameObjectToCheck.GetComponent<QueenAntScript>().TakeDamage(weaponInfo.baseDamage);
        }
    }
}