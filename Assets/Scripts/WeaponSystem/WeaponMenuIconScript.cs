using UnityEngine;
using UnityEngine.UI;

public class WeaponMenuIconScript : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private Image iconBackground;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject iconVisibility;
    [SerializeField] private GameObject buttonObj;

    public BaseWeaponSO Weapon { get; private set; }
    private WeaponManager weaponManager;
    private Button button;

    private void OnStart() {
        weaponManager = FindFirstObjectByType<WeaponManager>();
        button = buttonObj.GetComponent<Button>();
        DisableButtonInteraction();
    }

    public void SetWeapon(BaseWeaponSO _weapon, bool playerHasWeapon) {
        Weapon = _weapon;
        icon.sprite = Weapon.weaponIcon;
        OnStart();
        ToggleVisibility(playerHasWeapon);
    }

    public void ToggleVisibility(bool playerHasWeapon) {
        if (playerHasWeapon) {
            iconVisibility.SetActive(false);
            buttonObj.SetActive(true);
        }
        else {
            iconVisibility.SetActive(true);
            buttonObj.SetActive(false);
        }
    }

    public void SetSelectedWeapon() {
        weaponManager.SetSelectedWeapon(Weapon);
    }

    public void OnButtonHover() {
        iconBackground.color = new Color32(41, 169, 36, 255);
        weaponManager.UpdateWeaponInfo(Weapon);
    }

    public void OnButtonHoverExit() {
        iconBackground.color = new Color32(255, 165, 0, 255);
    }

    public void AllowButtonInteraction() {
        button.interactable = true;
    }

    public void DisableButtonInteraction() {
        button.interactable = false;
    }

    public GameObject GetButton() {
        return buttonObj;
    }
}