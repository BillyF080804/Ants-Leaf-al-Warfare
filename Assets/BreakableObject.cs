using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private GameObject fixedObj;
    [SerializeField] private GameObject brokenObj;
    private WeaponScript damageHit;
    private Collider colliderSelf;
    [SerializeField] private Vector3 launchSpeed;

    private void Awake()
    {
        colliderSelf = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        damageHit = collision.gameObject.GetComponent<WeaponScript>();
        if(damageHit != null)
        {
            colliderSelf.enabled = false;
            Debug.Log("Break");
            CollisionBreak();
        }
        damageHit = null;
    }

    private void CollisionBreak()
    {
        fixedObj.SetActive(false);
        brokenObj.SetActive(true);
        foreach(Transform child in brokenObj.transform)
        {
            child.GetComponent<Rigidbody>().velocity += launchSpeed;
        }
        Destroy(brokenObj, 5f);
    }
}
