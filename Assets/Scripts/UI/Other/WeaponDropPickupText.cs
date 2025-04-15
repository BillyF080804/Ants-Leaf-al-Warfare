using UnityEngine;

public class WeaponDropPickupText : MonoBehaviour {
    private void Start() {
        GetComponent<MoveUI>().StartMoveUI(LerpType.In, new Vector2(-25, 0), new Vector2(-25, 150), 1.5f);
        GetComponent<FadeScript>().FadeOutUI(1.5f);
        Destroy(gameObject, 1.5f);
    }
}