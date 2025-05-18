using UnityEngine;
using UnityEngine.Events;

public class BreakableObject : MonoBehaviour
{
    [Header("Assignments")]
    [SerializeField] private GameObject fixedObj;
    [SerializeField] private GameObject brokenObj;
    private WeaponScript damageHit;
    private Collider colliderSelf;

    [Header("Settings")]
    [SerializeField] private bool isRbody;
    [SerializeField] private Vector3 launchSpeed;
    public UnityEvent breakEvent;


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
            if(isRbody)
            {
                CollisionBreak();
            }
            else
            {
                breakEvent.Invoke();
            }

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
