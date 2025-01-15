using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponScript : MonoBehaviour {
    public BaseWeaponSO weaponInfo { private get; set; }

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
            Debug.Log(collider.name + " damage dealt = " + weaponInfo.baseDamage);
            collider.GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.explosionPower, transform.position, weaponInfo.explosionRange, 3, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        if (weaponInfo.explosive && weaponInfo.explodeOnImpact) {
            Explode();
        }
    }
}