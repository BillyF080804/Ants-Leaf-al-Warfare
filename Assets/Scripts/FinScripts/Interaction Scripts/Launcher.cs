using UnityEngine;
using UnityEngine.Events;
public class Launcher : MonoBehaviour {
    [Header("Setup")]
    [SerializeField] private Vector3 launchForce;
    [SerializeField] private UnityEvent launchEvent;
    private Ant currentAnt;
    private Rigidbody currentRbody;

    private void OnTriggerEnter(Collider other) {
        currentAnt = other.GetComponent<Ant>();
        currentRbody = currentAnt.gameObject.GetComponent<Rigidbody>();
        if (currentAnt != null && currentRbody != null) {
            LaunchLeaf();
        }
        currentAnt = null;
        currentRbody = null;
    }

    private void LaunchLeaf() {
        currentRbody.velocity = Vector3.zero;
        currentRbody.AddForce(launchForce, ForceMode.Impulse);
        if (launchEvent != null) {
            launchEvent.Invoke();
        }
    }
}
