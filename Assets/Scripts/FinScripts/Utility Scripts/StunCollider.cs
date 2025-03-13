using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunCollider : MonoBehaviour
{
    [SerializeField] private FlyTrapHazard trapHazard;
    private WeaponScript weaponObject;


    private void OnTriggerEnter(Collider other)
    {
        weaponObject = other.GetComponent<WeaponScript>();
        if (weaponObject != null)
        {
            trapHazard.GetHit();
            Debug.Log("Hit");
            Destroy(weaponObject.gameObject);
        }
        weaponObject = null;
    }
}
