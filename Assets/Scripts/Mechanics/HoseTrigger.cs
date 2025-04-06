using UnityEngine;

public class HoseTrigger : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            transform.GetComponentInParent<Hose>().AddAnt(other.GetComponent<Rigidbody>());
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            transform.GetComponentInParent<Hose>().RemoveAnt(other.GetComponent<Rigidbody>());
        }
    }
}