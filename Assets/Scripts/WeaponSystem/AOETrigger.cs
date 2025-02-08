using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AOETrigger : MonoBehaviour {
    private BaseWeaponSO weaponInfo;
    private List<Ant> targets = new List<Ant>();
    private float timer = 0.0f;

    public void SetWeaponInfo(BaseWeaponSO _weaponInfo) {
        weaponInfo = _weaponInfo;
        transform.localScale = new Vector3(weaponInfo.lingerEffectRange, weaponInfo.lingerEffectRange, weaponInfo.lingerEffectRange);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Player")) {
            targets.Add(collider.GetComponent<Ant>());

            if (CheckIsValid() && weaponInfo.slowMovement == true) {
                collider.GetComponent<Ant>().SlowMovement(weaponInfo.slowMultiplier);
            }
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Player") && targets.Contains(collider.GetComponent<Ant>())) {
            targets.Remove(collider.GetComponent<Ant>());
            collider.GetComponent<Ant>().ResetMovement(weaponInfo.slowMultiplier);
        }
    }

    private bool CheckIsValid() {
        if (weaponInfo != null && weaponInfo.hasVFX == true && weaponInfo.lingerEffect == true) {
            return true;
        }
        else {
            return false;
        }
    }

    private void Update() { 
        if (CheckIsValid() && weaponInfo.dealDamage == true) {
            timer += Time.deltaTime;

            if (timer >= 1.0f) {
                timer = 0.0f;

                foreach (Ant ant in targets) {
                    ant.TakeDamage(weaponInfo.damagePerSecond);
                }
            }
        }
    }

    private void OnDestroy() {
        foreach (Ant ant in targets) {
            ant.ResetMovement(weaponInfo.slowMultiplier);
        }
    }
}