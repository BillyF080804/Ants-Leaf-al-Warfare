using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Launcher : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Vector3 launchForce;
    [SerializeField] private UnityEvent launchEvent;
    private AntScript currentAnt;
    private Rigidbody currentRbody;

    private void OnTriggerEnter(Collider other)
    {
        currentAnt = other.GetComponent<AntScript>();
        currentRbody = currentAnt.gameObject.GetComponent<Rigidbody>();
        if (currentAnt != null && currentRbody != null)
        {
                LaunchLeaf();
        }
        currentAnt = null;
        currentRbody = null;
    }

    private void LaunchLeaf()
    {
        currentRbody.velocity = Vector3.zero;
        currentRbody.AddForce(launchForce, ForceMode.Impulse);
        if (launchEvent != null)
        {
            launchEvent.Invoke();
        }
    }
}
