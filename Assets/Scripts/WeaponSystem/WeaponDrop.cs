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
    private AudioSource audioSource;

    private void Awake() {
        turnManager = FindFirstObjectByType<TurnManager>();
        weaponDropSystem = FindFirstObjectByType<WeaponDropSystem>();
        audioSource = weaponDropSystem.GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            CollectDrop(collision.gameObject.GetComponent<Ant>());
        }
    }

    //Change drop to medkit
    public void SetDropMedkit(int medkitHealthToHeal) {
        crate.SetActive(false);
        medkit.SetActive(true);
        medkitHealth = medkitHealthToHeal;
    }

    //Change drop to weapon
    public void SetDropWeapon(BaseWeaponSO _weapon) {
        crate.SetActive(true);
        medkit.SetActive(false);
        weapon = _weapon;
    }

    //Called when player collides with drop
    private void CollectDrop(Ant ant) {
        int playerNum = int.Parse(ant.ownedPlayer.ToString().Last().ToString());
        Player player = turnManager.PlayerList.Where(x => x.playerInfo.playerNum == playerNum).First();
        TMP_Text pickupText = Instantiate(weaponDropSystem.PickupTextPrefab, turnManager.MainCanvas.transform);

        if (audioSource != null && audioSource.clip != null) {
            audioSource.Play();
        }

        if (weapon != null) {
            player.AddNewWeapon(weapon); //Collect weapon
            pickupText.text = "+1 " + GetWeaponSpriteIcon(weapon.weaponName);
        }
        else {
            ant.HealDamage(medkitHealth); //Add health
            pickupText.text = "+" + medkitHealth + " Health";
            pickupText.color = Color.green;
        }

        Destroy(gameObject);
    }

    private string GetWeaponSpriteIcon(string weaponName) {
        switch (weaponName) {
            case "Dirt Shot":
                return "<sprite=0>";
            case "Slingshot":
                return "<sprite=1>";
            case "Acorn Grenade":
                return "<sprite=2>";
            case "Racket Hit":
                return "<sprite=3>";
            case "DEBUG EXPLOSIVE":
                return "<sprite=4>";
            case "Rock Slide":
                return "<sprite=5>";
            case "Acorn Rocket":
                return "<sprite=6>";
            case "Flamer":
                return "<sprite=7>";
            case "Stink Bug":
                return "<sprite=8>";
            case "Vine Spring":
                return "<sprite=9>";
            case "Hot Coal":
                return "<sprite=10>";
            case "Nettle Stinger":
                return "<sprite=12>";
            case "Air Blower":
                return "<sprite=13>";
            default:
                return "NULL";
        }
    }
}