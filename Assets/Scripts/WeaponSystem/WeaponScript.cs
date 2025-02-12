using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponScript : MonoBehaviour {
    private AOETrigger aoeTriggerAreaPrefab;
    private BaseWeaponSO weaponInfo;
    private CameraSystem cameraSystem;

    private void Awake() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
    }

    public void SetupWeapon(BaseWeaponSO weapon, Collider objectCollider, AOETrigger _aoeTriggerAreaPrefab) {
        Physics.IgnoreCollision(GetComponent<Collider>(), objectCollider, true);
        aoeTriggerAreaPrefab = _aoeTriggerAreaPrefab;
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

        if (weaponInfo.hasVFX == true && weaponInfo.vfxObject != null) {
            CreateVFX();
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

        foreach (Collider collider in colliders) {
            Debug.Log(collider.name + " damage dealt: " + weaponInfo.baseDamage);
            collider.GetComponent<Ant>().TakeDamage(weaponInfo.baseDamage);
            collider.GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.explosionPower, transform.position, weaponInfo.explosionRange, weaponInfo.upwardsModifier, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }

    private void CreateVFX() {
        GameObject vfxObj = Instantiate(weaponInfo.vfxObject, transform.position, Quaternion.identity);
        vfxObj.transform.localScale = new Vector3(weaponInfo.vfxSize, weaponInfo.vfxSize, weaponInfo.vfxSize);
        Destroy(vfxObj, weaponInfo.vfxDuration);

        AOETrigger aoeTriggerArea = Instantiate(aoeTriggerAreaPrefab, transform.position, Quaternion.identity);
        aoeTriggerArea.SetWeaponInfo(weaponInfo);
        Destroy(aoeTriggerArea, weaponInfo.vfxDuration);
    }

    private void OnDestroy() {
        if (cameraSystem.CameraTarget == transform) {
            cameraSystem.SetCameraTarget(null);
        }
    }
}