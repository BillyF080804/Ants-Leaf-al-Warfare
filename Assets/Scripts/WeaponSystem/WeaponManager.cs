using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private GameObject weaponMenuUI;
    [SerializeField] private Transform weaponMenuUIGridArea;

    [Header("Prefabs")]
    [SerializeField] private WeaponMenuIconScript weaponIconPrefab;

    [Header("Default Player Weapons")]
    [SerializeField] private List<BaseWeaponSO> defaultPlayerWeapons = new List<BaseWeaponSO>();

    private bool uiMoving = false;
    private bool facingRight = true;
    private float aimStrength = 1.0f;
    private Vector2 aimPosition = Vector2.zero;

    public bool WeaponMenuOpen { get; private set; } = false;
    public BaseWeaponSO WeaponSelected { get; private set; }

    private TurnManager turnManager;
    private Coroutine aimCoroutine;
    private Coroutine aimStrengthCoroutine;
    private List<BaseWeaponSO> allWeapons = new List<BaseWeaponSO>();
    private List<WeaponMenuIconScript> weaponIcons = new List<WeaponMenuIconScript>();

    public Vector3 facingDirection;

    private void Start() { 
        turnManager = FindFirstObjectByType<TurnManager>();
        allWeapons = Resources.LoadAll<BaseWeaponSO>("").ToList();

        FillWeaponMenu();
        GivePlayerDefaultWeapons();
    }

    //Function for handling firing weapons
    public void FireWeapon(BaseWeaponSO weaponInfo, Transform playerPosition) {
        Vector3 spawnPos = new Vector3(aimPosition.x - 3, playerPosition.position.y, 0); //Also change the z on this to be the same as comment below.
        GameObject newWeapon = Instantiate(weaponInfo.weaponPrefab, playerPosition.position, Quaternion.identity);
        Rigidbody rb = newWeapon.GetComponent<Rigidbody>();
        WeaponScript weaponScript = newWeapon.GetComponent<WeaponScript>();
        Vector3 aimPos = new Vector3(aimPosition.x, aimPosition.y, 0); //Update the z value to be assigned from the inspector in a game manager script, with a z offset variable.

        //Calculate velocity & rotation
        Vector3 weaponDirection = aimPos - playerPosition.position;
        Vector3 weaponRotation = playerPosition.position - aimPos;

        //Set velocity
        Vector2 weaponVelocity = new Vector2(weaponDirection.x, weaponDirection.y).normalized * weaponInfo.weaponSpeed * aimStrength;
        rb.velocity = weaponVelocity;

        //Set rotation
        float originalRotation = Mathf.Atan2(weaponRotation.y, weaponRotation.x) * Mathf.Rad2Deg;
        newWeapon.transform.rotation = Quaternion.Euler(0, 0, originalRotation);

        //Set weapon values
        rb.useGravity = weaponInfo.useGravity;
        weaponScript.SetupWeapon(weaponInfo);
        
        if (weaponInfo.explosive && !weaponInfo.explodeOnImpact) {
            weaponScript.StartFuse();
        }

        WeaponSelected = null;
        turnManager.EndTurn();
    }

    //Handles stop/starting aiming coroutine
    public void AimWeapon(InputValue inputValue) {
        if (inputValue.Get<Vector2>() != Vector2.zero) {
            aimCoroutine = StartCoroutine(AimCoroutine(inputValue));
        }
        else {
            if (aimCoroutine != null) {
                StopCoroutine(aimCoroutine);
            }
        }
    }

    //Handles changing the aim whilst aiming buttons are held
    private IEnumerator AimCoroutine(InputValue inputValue) {
        Vector2 aimValue = inputValue.Get<Vector2>();

        if (aimValue.x < 0) {
            facingRight = false;
            aimPosition.x = turnManager.CurrentAntTurn.transform.position.x - 5;
        }
        else if (aimValue.x > 0) {
            facingRight = true;
            aimPosition.x = turnManager.CurrentAntTurn.transform.position.x + 5;
        }

        while (aimValue.y > 0) {
            aimPosition.y += 1 * Time.deltaTime;
            yield return null;
        }

        while (aimValue.y < 0) {
            aimPosition.y -= 1 * Time.deltaTime;
            yield return null;
        }
    }

    //Handles stop/starting aim strength coroutine
    public void AimStrength(InputValue inputValue) {
        if (inputValue.Get() != null) {
            aimStrengthCoroutine = StartCoroutine(AimStrengthCoroutine(inputValue));
        }
        else {
            if (aimStrengthCoroutine != null) {
                StopCoroutine(aimStrengthCoroutine);
            }
        }
    }

    //Used to increase the aim strength whilst button is held
    private IEnumerator AimStrengthCoroutine(InputValue inputValue) {
        float strengthValue = inputValue.Get<float>();

        while (strengthValue > 0) {
            aimStrength += 0.25f * Time.deltaTime;
            yield return null;
        }

        while (strengthValue < 0) {
            aimStrength -= 0.25f * Time.deltaTime;
            yield return null;
        }

        aimStrength = Mathf.Clamp(aimStrength, 0.1f, 2.0f);
        float arrowSize = aimArrowDefaultSize * aimStrength;
        aimArrow.transform.localScale = new Vector3(arrowSize, arrowSize, arrowSize);
    }

    //Resets the aim to a default position
    private void ResetAimPosition() {
        facingRight = true;
        aimPosition = turnManager.CurrentAntTurn.transform.position;
        aimPosition.x += 2;
        aimPosition.y += 2;
    }

    //Used to either open or close the weapon menu
    public void WeaponMenu() {
        if (WeaponMenuOpen == false && uiMoving == false && WeaponSelected != null) {
            WeaponSelected = null;
            aimArrow.SetActive(false);
        }
        else if (WeaponMenuOpen == true && uiMoving == false) {
            uiMoving = true;
            StartCoroutine(CloseWeaponMenuCoroutine());
        }
        else if (WeaponMenuOpen == false && uiMoving == false) {
            uiMoving = true;
            WeaponMenuOpen = true;
            StartCoroutine(OpenWeaponMenuCoroutine());
        }
    }

    public void ForceCloseWeaponMenu() {
        StartCoroutine(ForceCloseWeaponMenuCoroutine());
    }

    //Called at the end of a turn. Forces the weapon menu to close.
    private IEnumerator ForceCloseWeaponMenuCoroutine() {
        if (uiMoving == true) {
            yield return new WaitUntil(() => uiMoving == false);
        }

        if (WeaponMenuOpen == true) {
            StartCoroutine(CloseWeaponMenuCoroutine());
        }
    }

    //Closes the queen ant health ui and opens the weapons menu
    private IEnumerator OpenWeaponMenuCoroutine() {
        foreach (GameObject queenAntHealthUI in turnManager.QueenHealthUI) {
            RectTransform rect = queenAntHealthUI.GetComponent<RectTransform>();

            if (rect.localPosition.y > 0) {
                queenAntHealthUI.GetComponent<MoveUI>().StartMoveUI(LerpType.InBack, queenAntHealthUI, new Vector2(rect.localPosition.x, rect.localPosition.y), new Vector2(rect.localPosition.x, 650), 1.0f);
            }
            else {
                queenAntHealthUI.GetComponent<MoveUI>().StartMoveUI(LerpType.InBack, queenAntHealthUI, new Vector2(rect.localPosition.x, rect.localPosition.y), new Vector2(rect.localPosition.x, -650), 1.0f);
            }
        }

        yield return new WaitUntil(() => turnManager.QueenHealthUI[0].GetComponent<RectTransform>().localPosition.y > 640);

        foreach (GameObject queenHealthUI in turnManager.QueenHealthUI) {
            queenHealthUI.SetActive(false);
        }

        CheckAllIcons();
        weaponMenuUI.SetActive(true);
        weaponMenuUI.GetComponent<MoveUI>().StartMoveUI(LerpType.OutBack, weaponMenuUI, new Vector2(-500, 50), new Vector2(50, 50), 1.0f);

        yield return new WaitUntil(() => weaponMenuUI.GetComponent<RectTransform>().anchoredPosition == new Vector2(50, 50));
        uiMoving = false;
    }

    //Closes the weapons menu and opens the queen ant health UI
    private IEnumerator CloseWeaponMenuCoroutine() {
        weaponMenuUI.GetComponent<MoveUI>().StartMoveUI(LerpType.InBack, weaponMenuUI, new Vector2(50, 50), new Vector2(-500, 50), 1.0f);

        yield return new WaitUntil(() => weaponMenuUI.GetComponent<RectTransform>().anchoredPosition == new Vector2(-500, 50));
        weaponMenuUI.SetActive(false);
        WeaponMenuOpen = false;

        foreach (GameObject queenAntHealthUI in turnManager.QueenHealthUI) {
            RectTransform rect = queenAntHealthUI.GetComponent<RectTransform>();
            queenAntHealthUI.SetActive(true);

            if (rect.localPosition.y > 0) {
                queenAntHealthUI.GetComponent<MoveUI>().StartMoveUI(LerpType.OutBack, queenAntHealthUI, new Vector2(rect.localPosition.x, rect.localPosition.y), new Vector2(rect.localPosition.x, 415), 1.0f);
            }
            else {
                queenAntHealthUI.GetComponent<MoveUI>().StartMoveUI(LerpType.OutBack, queenAntHealthUI, new Vector2(rect.localPosition.x, rect.localPosition.y), new Vector2(rect.localPosition.x, -415), 1.0f);
            }
        }

        uiMoving = false;
    }

    private void FillWeaponMenu() {
        foreach (BaseWeaponSO weapon in allWeapons) {
            bool hasWeapon = defaultPlayerWeapons.Contains(weapon);
            WeaponMenuIconScript newIcon = Instantiate(weaponIconPrefab, weaponMenuUIGridArea);
            newIcon.GetComponent<WeaponMenuIconScript>().SetWeapon(weapon, hasWeapon);
            weaponIcons.Add(newIcon.GetComponent<WeaponMenuIconScript>());
        }
    }

    private void CheckAllIcons() {
        foreach (WeaponMenuIconScript icon in weaponIcons) {
            if (turnManager.CurrentPlayerTurn != null) {
                bool playerHasWeapon = turnManager.CurrentPlayerTurn.CurrentWeapons.Contains(icon.Weapon);
                icon.ToggleVisibility(playerHasWeapon);
            }
        }
    }

    private void GivePlayerDefaultWeapons() {
        foreach (Player player in turnManager.PlayerList) {
            foreach (BaseWeaponSO weapon in defaultPlayerWeapons) {
                player.AddNewWeapon(weapon);
            }
        }
    }

    public void SetSelectedWeapon(BaseWeaponSO weapon) {
        WeaponSelected = weapon;
        ResetAimPosition();
        StartCoroutine(CloseWeaponMenuCoroutine());
    }
}