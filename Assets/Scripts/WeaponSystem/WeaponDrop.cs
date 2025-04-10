using System.Linq;
using UnityEngine;

public class WeaponDrop : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private GameObject crate;
    [SerializeField] private GameObject medkit;

    private int medkitHealth = 0;
    private BaseWeaponSO weapon;
    private TurnManager turnManager;

    private void Awake() {
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            CollectDrop(collision.gameObject.GetComponent<Ant>());
        }
    }

    public void SetDropMedkit(int medkitHealthToHeal) {
        crate.SetActive(false);
        medkit.SetActive(true);
        medkitHealth = medkitHealthToHeal;
    }

    public void SetDropWeapon(BaseWeaponSO _weapon) {
        crate.SetActive(true);
        medkit.SetActive(false);
        weapon = _weapon;
    }

    private void CollectDrop(Ant ant) {
        int playerNum = int.Parse(ant.ownedPlayer.ToString().Last().ToString());
        Player player = turnManager.PlayerList.Where(x => x.playerInfo.playerNum == playerNum).First();

        if (weapon != null) {
            player.AddNewWeapon(weapon);
            Debug.Log("Added weapon " + weapon);
        }
        else {
            ant.HealDamage(medkitHealth);
            Debug.Log("Healed ant for " + medkitHealth);
        }

        Destroy(gameObject);
    }
}