using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponScript : MonoBehaviour {
    public BaseWeaponSO weaponInfo;
    private CameraSystem cameraSystem;
    private WeaponManager weaponManager;
    private TurnManager turnManager;
    private int numOfBounces = 0;

    private void Awake() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        weaponManager = FindFirstObjectByType<WeaponManager>();
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    //Called when weapon is created
    public void SetupWeapon(BaseWeaponSO weapon, Collider objectCollider) {
        if (objectCollider != null) {
            Physics.IgnoreCollision(GetComponent<Collider>(), objectCollider, true);
        }

        weaponInfo = weapon;
        Destroy(gameObject, 10.0f);
    }

    private void OnCollisionEnter(Collision collision) {
        if (weaponInfo.explosive && weaponInfo.explodeOnImpact) {
            Explode(); //explode weapon
        }
        else if (collision.gameObject.CompareTag("Player") && weaponInfo.explosive == false) {
            collision.gameObject.GetComponent<Ant>().TakeDamage(weaponInfo.baseDamage); //deal damage

            if (weaponInfo.hasVFX == true && weaponInfo.vfxObject != null) {
                CreateVFX();
            }

            if (weaponInfo.weaponEffect != null) {
                weaponInfo.weaponEffect.GetComponent<EffectScript>().AddEffect(collision.gameObject.GetComponent<Ant>()); //add effect to ant
            }

            if (weaponInfo.cameraShakeOnImpact) {
                cameraSystem.StartCameraShake(weaponInfo.cameraShakeDuration, weaponInfo.cameraShakeIntensity); //camera shake
            }

            if (weaponInfo.hasSounds && weaponInfo.impactSound != null) {
                weaponManager.AudioPlayer.ChangeClip(weaponInfo.impactSound); //create sfx
                weaponManager.AudioPlayer.PlayClip();
            }

            if (weaponInfo.hasKnockback) {
                DealKnockback(collision.gameObject); //Deal knockback to ant
            }

            Destroy(gameObject);
        }
        else if (weaponInfo.explosive == false) {
            numOfBounces++;

            if (weaponInfo.cameraShakeOnImpact) {
                cameraSystem.StartCameraShake(weaponInfo.cameraShakeDuration, weaponInfo.cameraShakeIntensity); //camera shake
            }

            if (weaponInfo.hasSounds && weaponInfo.impactSound != null) {
                weaponManager.AudioPlayer.ChangeClip(weaponInfo.impactSound); //create sfx
                weaponManager.AudioPlayer.PlayClip();
            }

            if (numOfBounces > weaponInfo.maxNumOfBounces) {
                Destroy(gameObject); //destroy object after x bounces
            }
        }
    }

    public void StartFuse() {
        StartCoroutine(FuseTimer());
    }

    //Fuse timer
    private IEnumerator FuseTimer() {
        float fuseTimer = weaponInfo.fuseTimer;

        while (fuseTimer > 0) {
            fuseTimer -= Time.deltaTime;
            yield return null;
        }

        Explode();
    }

    //Explode weapon
    private void Explode() {
        Collider[] collidersOne = Physics.OverlapSphere(transform.position, weaponInfo.explosionRange).Where(x => x.CompareTag("Player")).ToArray();
        Collider[] collidersTwo = Physics.OverlapSphere(transform.position, weaponInfo.explosionRange * 1.5f).Where(x => x.CompareTag("Player")).ToArray();

        foreach (Collider collider in collidersOne) {
            collider.GetComponent<Ant>().TakeDamage(weaponInfo.baseDamage); //deal damage
            collider.GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.explosionPower, transform.position, weaponInfo.explosionRange, weaponInfo.upwardsModifier, ForceMode.Impulse); //weapon knockback
            turnManager.AddMovingAnt(collider.GetComponent<Ant>());

            if (weaponInfo.weaponEffect != null) {
                weaponInfo.weaponEffect.GetComponent<EffectScript>().AddEffect(collider.GetComponent<Ant>()); //Add effect to ants
            }
        }

        foreach (Collider collider in collidersTwo) {
            if (!collidersOne.Contains(collider)) {
                collider.GetComponent<Ant>().TakeDamage(Mathf.FloorToInt(weaponInfo.baseDamage * 0.5f)); //deal damage to ants - weaker cause further from explosion
                collider.GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.explosionPower * 0.5f, transform.position, weaponInfo.explosionRange, weaponInfo.upwardsModifier, ForceMode.Impulse); //deal knockback to ants - weaker cause further from explosion
                turnManager.AddMovingAnt(collider.GetComponent<Ant>());

                if (weaponInfo.weaponEffect != null) {
                    weaponInfo.weaponEffect.GetComponent<EffectScript>().AddEffect(collider.GetComponent<Ant>()); //Add effect to ants
                }
            }
        }

        if (weaponInfo.hasVFX == true && weaponInfo.vfxObject != null) {
            CreateVFX();
        }

        if (weaponInfo.cameraShakeOnImpact) {
            cameraSystem.StartCameraShake(weaponInfo.cameraShakeDuration, weaponInfo.cameraShakeIntensity); //camera shake
        }

        if (weaponInfo.hasSounds && weaponInfo.impactSound != null) {
            weaponManager.AudioPlayer.ChangeClip(weaponInfo.impactSound); //create sfx
            weaponManager.AudioPlayer.PlayClip();
        }

        Destroy(gameObject);
    }

    private void DealKnockback(GameObject objectToKnockback) {
        objectToKnockback.GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.knockbackStrength, transform.position, 0, weaponInfo.upwardsModifier, ForceMode.Impulse); //knockback for ants
        turnManager.AddMovingAnt(objectToKnockback.GetComponent<Ant>());
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