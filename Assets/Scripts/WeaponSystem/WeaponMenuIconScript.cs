using UnityEngine;
using UnityEngine.UI;

public class WeaponMenuIconScript : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private GameObject iconVisibility;
    [SerializeField] private GameObject button;

    public BaseWeaponSO Weapon { get; private set; }
    private WeaponManager weaponManager;

    private void Start() {
        weaponManager = FindFirstObjectByType<WeaponManager>();
    }

    public void SetWeapon(BaseWeaponSO _weapon, bool playerHasWeapon) {
        Weapon = _weapon;
        icon.sprite = Weapon.weaponIcon;
        ToggleVisibility(playerHasWeapon);
    }

    public void ToggleVisibility(bool playerHasWeapon) {
        if (playerHasWeapon) {
            iconVisibility.SetActive(false);
            button.SetActive(true);
        }
        else {
            iconVisibility.SetActive(true);
            button.SetActive(false);
        }
    }

    public void SetSelectedWeapon() {
        weaponManager.SetSelectedWeapon(Weapon);
    }

    public void OnButtonHover() {
        weaponManager.UpdateWeaponInfo(Weapon);
    }
}