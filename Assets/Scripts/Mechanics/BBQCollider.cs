using UnityEngine;

public class BBQCollider : MonoBehaviour {
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            transform.GetComponentInParent<BBQScript>().AddBurnerAnt(collision.gameObject.GetComponent<Ant>());
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            transform.GetComponentInParent<BBQScript>().RemoveBurnerAnt(collision.gameObject.GetComponent<Ant>());
        }
    }
}