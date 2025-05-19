using UnityEngine;

public class Pebble : MonoBehaviour {
    [Header("References")]
    [SerializeField] string GroundTag;
    Rigidbody pebbleBody;
    [Header("Variables")]
    public float inactiveTime;
    [SerializeField] private float maxInactiveTime;
    public Vector3 minimumSpeed;


    private void Awake() {
        // Getting reference to own Rigidbody
        pebbleBody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) {
        //If statement to see if the Ground is what has been collided with or not
        int LayerIgnore = LayerMask.NameToLayer(GroundTag);
        if (collision.gameObject.layer != LayerIgnore) {
            pebbleBody.isKinematic = true;
        }
    }

    private void FixedUpdate() {
        if (pebbleBody.isKinematic) {
            // Checking if the RigidBody is moving slow enough to be considered "inactive" if it's still active
            if (pebbleBody.velocity.x <= minimumSpeed.x && pebbleBody.velocity.y <= minimumSpeed.y) {
                InActiveChecker();
            }
            else {
                inactiveTime = 0;
            }
        }

    }

    private void InActiveChecker() {
        inactiveTime = Time.deltaTime;
        if (inactiveTime >= maxInactiveTime) {
            pebbleBody.isKinematic = false;
        }
    }
}
