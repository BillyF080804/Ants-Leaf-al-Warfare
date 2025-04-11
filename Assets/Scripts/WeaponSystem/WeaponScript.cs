using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponScript : MonoBehaviour {
    public BaseWeaponSO weaponInfo;
    private CameraSystem cameraSystem;

    private void Awake() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
    }

    public void SetupWeapon(BaseWeaponSO weapon, Collider objectCollider) {
        if (objectCollider != null) {
            Physics.IgnoreCollision(GetComponent<Collider>(), objectCollider, true);
        }

        weaponInfo = weapon;
        Destroy(gameObject, 10.0f);
    }

    private void OnCollisionEnter(Collision collision) {
        if (weaponInfo.explosive && weaponInfo.explodeOnImpact) {
            Explode();
        }
        else if (collision.gameObject.CompareTag("Player") && weaponInfo.explosive == false) {
            collision.gameObject.GetComponent<Ant>().TakeDamage(weaponInfo.baseDamage);

            if (weaponInfo.hasVFX == true && weaponInfo.vfxObject != null) {
                CreateVFX();
            }

            if (weaponInfo.weaponEffect != null) {
                weaponInfo.weaponEffect.GetComponent<EffectScript>().AddEffect(collision.gameObject.GetComponent<Ant>());
            }

            if (weaponInfo.cameraShakeOnImpact) {
                cameraSystem.StartCameraShake(weaponInfo.cameraShakeDuration, weaponInfo.cameraShakeIntensity);
            }

            Destroy(gameObject);
        }
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
        Collider[] collidersOne = Physics.OverlapSphere(transform.position, weaponInfo.explosionRange).Where(x => x.CompareTag("Player")).ToArray();
        Collider[] collidersTwo = Physics.OverlapSphere(transform.position, weaponInfo.explosionRange * 1.5f).Where(x => x.CompareTag("Player")).ToArray();

        foreach (Collider collider in collidersOne) {
            collider.GetComponent<Ant>().TakeDamage(weaponInfo.baseDamage);
            collider.GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.explosionPower, transform.position, weaponInfo.explosionRange, weaponInfo.upwardsModifier, ForceMode.Impulse);

            if (weaponInfo.weaponEffect != null) {
                weaponInfo.weaponEffect.GetComponent<EffectScript>().AddEffect(collider.GetComponent<Ant>());
            }
        }

        foreach (Collider collider in collidersTwo) {
            if (!collidersOne.Contains(collider)) {
                collider.GetComponent<Ant>().TakeDamage(Mathf.FloorToInt(weaponInfo.baseDamage * 0.5f));
                collider.GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.explosionPower * 0.5f, transform.position, weaponInfo.explosionRange, weaponInfo.upwardsModifier, ForceMode.Impulse);

                if (weaponInfo.weaponEffect != null) {
                    weaponInfo.weaponEffect.GetComponent<EffectScript>().AddEffect(collider.GetComponent<Ant>());
                }
            }
        }

        if (weaponInfo.hasVFX == true && weaponInfo.vfxObject != null) {
            CreateVFX();
        }

        if (weaponInfo.cameraShakeOnImpact) {
            cameraSystem.StartCameraShake(weaponInfo.cameraShakeDuration, weaponInfo.cameraShakeIntensity);
        }

        Destroy(gameObject);
    }

    public void CreateVFX() {
        GameObject vfxObj = Instantiate(weaponInfo.vfxObject, transform.position, Quaternion.identity);
        vfxObj.transform.localScale = new Vector3(weaponInfo.vfxSize, weaponInfo.vfxSize, weaponInfo.vfxSize);
        Destroy(vfxObj, weaponInfo.vfxDuration);
    }

    private void OnDestroy() {
        if (cameraSystem.CameraTarget == transform) {
            cameraSystem.SetCameraTarget(transform.position);
            cameraSystem.CameraDelay(weaponInfo.cameraDelay);
        }
    }
}