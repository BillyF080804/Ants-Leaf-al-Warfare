using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponScript : MonoBehaviour {
    private BaseWeaponSO weaponInfo;

    public void SetupWeapon(BaseWeaponSO weapon, Collider objectCollider) {
        Physics.IgnoreCollision(GetComponent<Collider>(), objectCollider, true);
        weaponInfo = weapon;
        Destroy(gameObject, 10.0f);
    }

    private void OnCollisionEnter(Collision collision) {
        if (weaponInfo.explosive && weaponInfo.explodeOnImpact) {
            Explode();
        }
        else if (collision.gameObject.CompareTag("Player")) {
            Debug.Log(collision.gameObject.name + " damage dealt: " + weaponInfo.baseDamage);
            collision.gameObject.GetComponent<Ant>().TakeDamage(weaponInfo.baseDamage);
        }

        Destroy(gameObject);
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

        if (weaponInfo.hasVFX == true && weaponInfo.vfxObject != null) {
            CreateVFX();
        }

        foreach (Collider collider in colliders) {
            Debug.Log(collider.name + " damage dealt: " + weaponInfo.baseDamage);
            collider.GetComponent<Ant>().TakeDamage(weaponInfo.baseDamage);
            collider.GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.explosionPower, transform.position, weaponInfo.explosionRange, 3, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }

    private void CreateVFX() {
        GameObject vfxObj = Instantiate(weaponInfo.vfxObject, transform.position, Quaternion.identity);
        vfxObj.transform.localScale = new Vector3(weaponInfo.vfxSize, weaponInfo.vfxSize, weaponInfo.vfxSize);
        Destroy(vfxObj, 2.5f);
    }
}