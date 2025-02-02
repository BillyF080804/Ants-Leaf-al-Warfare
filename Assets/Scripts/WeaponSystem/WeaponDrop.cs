using UnityEngine;

public class WeaponDrop : MonoBehaviour {
    private string dropType = string.Empty;
    private BaseWeaponSO weapon;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            CollectDrop(collision.gameObject);
        }
    }

    public void SetDropMedkit() {
        dropType = "Medkit";
    }

    public void SetDropWeapon(BaseWeaponSO _weapon) {
        dropType = "Weapon";
        weapon = _weapon;
    }

    private void CollectDrop(GameObject antObject) {
        Debug.Log(antObject.name + " picked up " + dropType + weapon);

        if (weapon != null) {
            //Add weapon on Player
        }
        else {
            //Add health on ant
        }

        Destroy(gameObject);
    }
}