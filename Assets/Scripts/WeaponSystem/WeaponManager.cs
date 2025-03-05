using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private RectTransform aimArrow;

    [Header("Weapon Menu UI")]
    [SerializeField] private GameObject weaponMenuUI;
    [SerializeField] private Transform weaponMenuUIGridArea;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text weaponDescriptionText;

    [Header("Prefabs")]
    [SerializeField] private WeaponMenuIconScript weaponIconPrefab;

    [Header("Default Player Weapons")]
    [SerializeField] private List<BaseWeaponSO> defaultPlayerWeapons = new List<BaseWeaponSO>();

    private bool canAim = true;
    private bool canFireWeapon = true;
    private float aimStrength = 1.0f;
    private float aimArrowDefaultSize = 2.0f;
    private float strengthValue = 0.0f;
    private Vector2 aimValue = Vector2.zero;
    private Vector2 aimPosition = Vector2.zero;

    public bool UIMoving { get; private set; } = false;
    public bool WeaponMenuOpen { get; private set; } = false;
    public bool WeaponsActive { get; private set; } = false;
    public BaseWeaponSO WeaponSelected { get; private set; }

    private CameraSystem cameraSystem;
    private TurnManager turnManager;
    private List<GameObject> activeWeapons = new List<GameObject>();
    private List<BaseWeaponSO> allWeapons = new List<BaseWeaponSO>();
    private List<WeaponMenuIconScript> weaponIcons = new List<WeaponMenuIconScript>();

    private void Start() { 
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        turnManager = FindFirstObjectByType<TurnManager>();
        allWeapons = Resources.LoadAll<BaseWeaponSO>("").ToList();
        aimArrowDefaultSize = aimArrow.localScale.x;

        FillWeaponMenu();
        GivePlayerDefaultWeapons();
    }

    //Function for handling firing weapons
    public void FireWeapon(BaseWeaponSO weaponInfo, Transform playerPosition) {
        if (canFireWeapon) {
            canFireWeapon = false;
            canAim = false;
            StartCoroutine(FireWeaponCoroutine(weaponInfo, playerPosition));
        }
    }

    private IEnumerator FireWeaponCoroutine(BaseWeaponSO weaponInfo, Transform playerPosition) {
        if (weaponInfo.isMultiShot == false) {
            OnShoot(weaponInfo, playerPosition);
        }
        else if (weaponInfo.isMultiShot == true) { 
            for (int i = 0; i < weaponInfo.numOfShots; i++) {
                OnShoot(weaponInfo, playerPosition);
                yield return new WaitForSeconds(weaponInfo.delayBetweenShots);
            }
        }

        if (weaponInfo.unlimitedUses == false) {
            turnManager.CurrentPlayerTurn.RemoveWeapon(weaponInfo);
        }

        WaitTillWeaponsFinished();
    }

    private void OnShoot(BaseWeaponSO weaponInfo, Transform playerPosition) {
        Vector3 spawnPos = new Vector3(playerPosition.position.x, playerPosition.position.y + 1, 0); //Also change the z on this to be the same as comment below.
        GameObject newWeapon = Instantiate(weaponInfo.weaponPrefab, spawnPos, Quaternion.identity);
        Rigidbody rb = newWeapon.GetComponent<Rigidbody>();
        WeaponScript weaponScript = newWeapon.GetComponent<WeaponScript>();
        Vector3 aimPos = new Vector3(aimPosition.x, aimPosition.y, 0); //Update the z value to be assigned from the inspector in a game manager script, with a z offset variable.

        if (weaponInfo.weaponRandomisation == true) {
            aimPos.y *= Random.Range(weaponInfo.minimumRandomness, weaponInfo.maximumRandomness);
        }

        //Calculate velocity & rotation
        Vector3 weaponDirection = aimPos - playerPosition.position;
        Vector3 weaponRotation = playerPosition.position - aimPos;

        //Set velocity
        Vector2 weaponVelocity = new Vector2(weaponDirection.x, weaponDirection.y).normalized * weaponInfo.weaponSpeed * aimStrength * (weaponInfo.weaponRandomisation == true ? Random.Range(weaponInfo.minimumRandomness, weaponInfo.maximumRandomness) : 1.0f);
        rb.velocity = weaponVelocity;

        //Set rotation
        float originalRotation = Mathf.Atan2(weaponRotation.y, weaponRotation.x) * Mathf.Rad2Deg;
        newWeapon.transform.rotation = Quaternion.Euler(0, 0, originalRotation);

        //Set weapon values
        rb.useGravity = weaponInfo.useGravity;
        weaponScript.SetupWeapon(weaponInfo, playerPosition.GetComponent<Collider>());
        activeWeapons.Add(newWeapon);
        WeaponsActive = true;
        cameraSystem.SetCameraTarget(newWeapon.transform);

        if (weaponInfo.explosive && !weaponInfo.explodeOnImpact) {
            weaponScript.StartFuse();
        }

        if (weaponInfo.cameraShakeOnFire) {
            cameraSystem.StartCameraShake(weaponInfo.cameraShakeDuration, weaponInfo.cameraShakeIntensity);
        }
    }

    public void UseMeleeWeapon(BaseWeaponSO weaponInfo, Transform playerPosition) {
        if (canFireWeapon) {
            canFireWeapon = false;
            canAim = false;
            Collider[] colliders = Physics.OverlapSphere(aimPosition, 2.5f).Where(x => x.CompareTag("Player") && x.gameObject != turnManager.CurrentAntTurn.gameObject).ToArray();

            if (colliders.Length > 0) {
                colliders.First().GetComponent<Ant>().TakeDamage(weaponInfo.baseDamage);
                colliders.First().GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.knockbackStrength, playerPosition.position, 0, weaponInfo.upwardsModifier, ForceMode.Impulse);

                if (weaponInfo.weaponEffect != null) {
                    weaponInfo.weaponEffect.AddEffect(colliders.First().GetComponent<Ant>());
                }
            }

            if (weaponInfo.cameraShakeOnFire) {
                cameraSystem.StartCameraShake(weaponInfo.cameraShakeDuration, weaponInfo.cameraShakeIntensity);
            }

            cameraSystem.CameraDelay(weaponInfo.cameraDelay);
            cameraSystem.SetCameraTarget(playerPosition.position);
            WaitTillWeaponsFinished();
        }
    }

    public void UseSprayWeapon(BaseWeaponSO weaponInfo, Transform playerPosition) {
        if (canFireWeapon) {
            canFireWeapon = false;
            canAim = false;
            Vector3 spawnPos = new Vector3(playerPosition.position.x, playerPosition.position.y, playerPosition.position.z);
            Vector3 scale = new Vector3(weaponInfo.sprayHeight, weaponInfo.sprayLength, weaponInfo.sprayLength);

            if (aimPosition.x < playerPosition.position.x) {
                spawnPos.x -= weaponInfo.sprayLength * 0.5f;
            }
            else {
                spawnPos.x += weaponInfo.sprayLength * 0.5f;
            }

            GameObject sprayArea = Instantiate(weaponInfo.sprayAreaObject, spawnPos, Quaternion.identity);
            sprayArea.transform.localScale = scale;
            sprayArea.transform.localRotation = aimArrow.localRotation;

            sprayArea.GetComponent<SprayAreaScript>().Setup(turnManager.CurrentAntTurn.GetComponent<Collider>(), weaponInfo, turnManager);

            if (weaponInfo.cameraShakeOnFire) {
                cameraSystem.StartCameraShake(weaponInfo.cameraShakeDuration, weaponInfo.cameraShakeIntensity);
            }

            activeWeapons.Add(sprayArea);
            WeaponsActive = true;
            cameraSystem.CameraDelay(weaponInfo.cameraDelay);
            cameraSystem.SetCameraTarget(playerPosition.position);
            WaitTillWeaponsFinished();
        }
    }

    public void WaitTillWeaponsFinished() {
        StartCoroutine(WaitTillWeaponsFinishedCoroutine());
    }

    private IEnumerator WaitTillWeaponsFinishedCoroutine() {
        aimArrow.gameObject.SetActive(false);
        yield return new WaitUntil(() => activeWeapons.Where(x => x != null).Count() == 0 && cameraSystem.CameraDelayActive == false);
        canFireWeapon = true;
        WeaponsActive = false;
        activeWeapons.Clear();
        turnManager.EndTurn();
    }

    //Handles setting aimValue
    public void AimWeapon(InputValue inputValue) {
        aimValue = inputValue.Get<Vector2>();
    }

    //Handles setting strengthValue
    public void AimStrength(InputValue inputValue) {
        strengthValue = inputValue.Get<float>();
    }

    private void ArrowAim() {
        if (aimValue.x < -0.25f) {
            aimPosition.x = turnManager.CurrentAntTurn.transform.position.x - 5;
        }
        else if (aimValue.x > 0.25f) {
            aimPosition.x = turnManager.CurrentAntTurn.transform.position.x + 5;
        }

        aimPosition.y += aimValue.y * Time.deltaTime;
    }

    private void ArrowStrength() {
        if (strengthValue > 0) {
            aimStrength += 0.25f * Time.deltaTime;
        }
        else if (strengthValue < 0) { 
            aimStrength -= 0.25f * Time.deltaTime;
        }
    }

    private void Update() {
        if (WeaponSelected != null && canAim == true) {
            ArrowAim();
            ArrowStrength();
            UpdateArrowSize();
            UpdateArrowAim();
        }
    }

    //Resets the aim to a default position
    private void ResetAimPosition() {
        aimPosition = turnManager.CurrentAntTurn.transform.position;
        aimPosition.x += 2;

        aimArrow.gameObject.SetActive(true);
        canAim = true;

        Vector3 arrowPos = turnManager.CurrentAntTurn.transform.position;
        arrowPos.x += 1;
        aimArrow.anchoredPosition = arrowPos;
        aimStrength = 1.0f;

        UpdateArrowAim();
        UpdateArrowSize();
    }

    private void UpdateArrowAim() {
        Vector3 aimRotation = (Vector3)aimPosition - aimArrow.position;
        aimArrow.localRotation = Quaternion.LookRotation(Vector3.forward, new Vector2(-aimRotation.y, aimRotation.x));
        aimArrow.localEulerAngles = new Vector3(0, 0, aimArrow.localEulerAngles.z - 90);

        if (aimPosition.x < turnManager.CurrentAntTurn.transform.position.x) {
            Vector3 arrowPos = turnManager.CurrentAntTurn.transform.position;
            arrowPos.x -= 1;
            aimArrow.anchoredPosition = arrowPos;
        }
        else {
            Vector3 arrowPos = turnManager.CurrentAntTurn.transform.position;
            arrowPos.x += 1;
            aimArrow.anchoredPosition = arrowPos;
        }
    }

    private void UpdateArrowSize() {
        aimStrength = Mathf.Clamp(aimStrength, 0.5f, 2.0f);
        float arrowSize = aimArrowDefaultSize * aimStrength;
        aimArrow.transform.localScale = new Vector3(arrowSize, arrowSize, arrowSize);
    }

    //Used to either open or close the weapon menu
    public void WeaponMenu() {
        if (WeaponMenuOpen == false && UIMoving == false && WeaponSelected != null) {
            WeaponSelected = null;
            aimArrow.gameObject.SetActive(false);
        }
        else if (WeaponMenuOpen == true && UIMoving == false) {
            UIMoving = true;
            StartCoroutine(CloseWeaponMenuCoroutine());
        }
        else if (WeaponMenuOpen == false && UIMoving == false) {
            UIMoving = true;
            WeaponMenuOpen = true;
            turnManager.CurrentAntTurn.StopMovement();
            StartCoroutine(OpenWeaponMenuCoroutine());
        }
    }

    //Called when a turn is ended
    public void EndTurn() {
        WeaponSelected = null;
        aimStrength = 0.0f;
        aimValue = Vector2.zero;
        aimArrow.gameObject.SetActive(false);

        StartCoroutine(ForceCloseWeaponMenuCoroutine());
    }

    //Called at the end of a turn. Forces the weapon menu to close.
    private IEnumerator ForceCloseWeaponMenuCoroutine() {
        if (UIMoving == true) {
            yield return new WaitUntil(() => UIMoving == false);
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
        turnManager.CurrentPlayerTurn.GetEventSystem().SetSelectedGameObject(weaponIcons.Where(x => x.Interactable == true).First().GetButton());
        weaponMenuUI.GetComponent<MoveUI>().StartMoveUI(LerpType.OutBack, weaponMenuUI, new Vector2(-750, 50), new Vector2(50, 50), 1.0f);

        yield return new WaitUntil(() => weaponMenuUI.GetComponent<RectTransform>().anchoredPosition == new Vector2(50, 50));
        EnableIconInteraction();
        UIMoving = false;
    }

    //Closes the weapons menu and opens the queen ant health UI
    private IEnumerator CloseWeaponMenuCoroutine() {
        DisableIconInteraction();
        weaponMenuUI.GetComponent<MoveUI>().StartMoveUI(LerpType.InBack, weaponMenuUI, new Vector2(50, 50), new Vector2(-750, 50), 1.0f);

        yield return new WaitUntil(() => weaponMenuUI.GetComponent<RectTransform>().anchoredPosition == new Vector2(-750, 50));
        weaponMenuUI.SetActive(false);
        WeaponMenuOpen = false;

        foreach (GameObject queenAntHealthUI in turnManager.QueenHealthUI) {
            RectTransform rect = queenAntHealthUI.GetComponent<RectTransform>();
            queenAntHealthUI.SetActive(true);

            if (rect.localPosition.y > 0) {
                queenAntHealthUI.GetComponent<MoveUI>().StartMoveUI(LerpType.OutBack, queenAntHealthUI, new Vector2(rect.localPosition.x, rect.localPosition.y), new Vector2(rect.localPosition.x, 445), 1.0f);
            }
            else {
                queenAntHealthUI.GetComponent<MoveUI>().StartMoveUI(LerpType.OutBack, queenAntHealthUI, new Vector2(rect.localPosition.x, rect.localPosition.y), new Vector2(rect.localPosition.x, -445), 1.0f);
            }
        }

        UIMoving = false;
    }

    private void FillWeaponMenu() {
        foreach (BaseWeaponSO weapon in allWeapons) {
            WeaponMenuIconScript newIcon = Instantiate(weaponIconPrefab, weaponMenuUIGridArea);
            weaponIcons.Add(newIcon.GetComponent<WeaponMenuIconScript>());

            bool hasWeapon = defaultPlayerWeapons.Contains(weapon);
            int weaponCount = -1;

            if (weapon.unlimitedUses == false) {
                weaponCount = defaultPlayerWeapons.Where(x => x == weapon).Count();
            }

            newIcon.GetComponent<WeaponMenuIconScript>().SetWeapon(weapon, hasWeapon, weaponCount);
        }
    }

    private void CheckAllIcons() {
        foreach (WeaponMenuIconScript icon in weaponIcons) {
            if (turnManager.CurrentPlayerTurn != null) {
                bool playerHasWeapon = turnManager.CurrentPlayerTurn.CurrentWeapons.Contains(icon.Weapon);
                int weaponCount = -1;

                if (icon.Weapon.unlimitedUses == false) {
                    weaponCount = turnManager.CurrentPlayerTurn.CurrentWeapons.Where(x => x == icon.Weapon).Count();
                }

                icon.ToggleVisibility(playerHasWeapon, weaponCount);
            }
        }
    }

    private void EnableIconInteraction() {
        foreach (WeaponMenuIconScript icon in weaponIcons) {
            icon.AllowButtonInteraction();
        }
    }

    private void DisableIconInteraction() {
        foreach (WeaponMenuIconScript icon in weaponIcons) {
            icon.DisableButtonInteraction();
        }
    }

    private void GivePlayerDefaultWeapons() {
        foreach (Player player in turnManager.PlayerList) {
            foreach (BaseWeaponSO weapon in defaultPlayerWeapons) {
                player.AddNewWeapon(weapon);
            }
        }
    }

    public void UpdateWeaponInfo(BaseWeaponSO weapon) {
        weaponNameText.text = weapon.weaponName;
        weaponDescriptionText.text = weapon.weaponDescription;
    }

    public void SetSelectedWeapon(BaseWeaponSO weapon) {
        WeaponSelected = weapon;
        ResetAimPosition();
        StartCoroutine(CloseWeaponMenuCoroutine());
    }
}