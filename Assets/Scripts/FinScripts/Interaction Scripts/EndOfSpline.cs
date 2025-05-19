using UnityEngine;

public class EndOfSpline : MonoBehaviour {
    AttachToSpline atSpline;

    private void Awake() {
        atSpline = GetComponentInParent<AttachToSpline>();

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == atSpline.currentAnt.gameObject) {
            atSpline.DetachObject();
        }
    }
}
