using System.Linq;
using TMPro;
using UnityEngine;

public class WeaponDrop : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private GameObject crate;
    [SerializeField] private GameObject medkit;

    private int medkitHealth = 0;
    private BaseWeaponSO weapon;
    private TurnManager turnManager;
    private WeaponDropSystem weaponDropSystem;

    private void Awake() {
        turnManager = FindFirstObjectByType<TurnManager>();
        weaponDropSystem = FindFirstObjectByType<WeaponDropSystem>();
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
        TMP_Text pickupText = Instantiate(weaponDropSystem.PickupTextPrefab, turnManager.MainCanvas.transform);

        if (weapon != null) {
            player.AddNewWeapon(weapon);
            pickupText.text = "+1 " + weapon.weaponName;
        }
        else {
            ant.HealDamage(medkitHealth);
            pickupText.text = "+" + medkitHealth + " Health";
            pickupText.color = Color.green;
        }

        Destroy(gameObject);
    }
}