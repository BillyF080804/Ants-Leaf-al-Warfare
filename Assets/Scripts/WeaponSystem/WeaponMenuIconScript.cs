using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponMenuIconScript : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private Image iconBackground;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject iconVisibility;
    [SerializeField] private TMP_Text weaponCountText;
    [SerializeField] private GameObject buttonObj;

    public bool Interactable { get; private set; } = true;
    public BaseWeaponSO Weapon { get; private set; }
    private WeaponManager weaponManager;
    private Button button;

    private void OnStart() {
        weaponManager = FindFirstObjectByType<WeaponManager>();
        button = buttonObj.GetComponent<Button>();
        DisableButtonInteraction();
    }

    public void SetWeapon(BaseWeaponSO _weapon, bool playerHasWeapon, int weaponCount) {
        Weapon = _weapon;
        icon.sprite = Weapon.weaponIcon;
        OnStart();
        ToggleVisibility(playerHasWeapon, weaponCount);
    }

    public void ToggleVisibility(bool playerHasWeapon, int weaponCount) {
        if (playerHasWeapon) {
            Interactable = true;
            iconVisibility.SetActive(false);
            buttonObj.SetActive(true);
        }
        else {
            Interactable = false;
            iconVisibility.SetActive(true);
            buttonObj.SetActive(false);
        }

        if (weaponCount == -1) {
            weaponCountText.gameObject.SetActive(false);
        }
        else {
            weaponCountText.gameObject.SetActive(true);
            weaponCountText.text = weaponCount.ToString();
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