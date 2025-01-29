using UnityEngine;

public class WeaponDrop : MonoBehaviour {
    private string dropType = string.Empty;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            CollectDrop();
        }
    }

    public void SetDropType(string _dropType) {
        dropType = _dropType;
    }

    private void CollectDrop() {
        Debug.Log(dropType);
        Destroy(gameObject);
    }
}