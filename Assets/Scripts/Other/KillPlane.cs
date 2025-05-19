using UnityEngine;

public class KillPlane : MonoBehaviour {


    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<Ant>()) {
            collision.gameObject.GetComponent<Ant>().TakeDamage(100);
        }
        else {
            Destroy(collision.gameObject);
        }
    }
}
