using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour {
    [Header("UI/Settings")]
    [SerializeField] private RectTransform aimArrow;
    [SerializeField] private float aimRotationSpeed = 50f;

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
    private bool fireHeld = false;
    private bool aimingLeft = false;
    private float aimStrength = 0.5f;
    private float aimArrowDefaultSize = 0.5f;
    private float aimRot = 0.0f;
    private Vector2 aimValue = Vector2.zero;
    private Vector2 aimPosition = Vector2.zero;

    private BaseWeaponSO tempWeaponInfo;
    private Transform tempPlayerPosition;

    public delegate void OnOpenWeaponsMenu();
    public static OnOpenWeaponsMenu onOpenWeaponsMenu;
    public delegate void OnCloseWeaponsMenu();
    public static OnCloseWeaponsMenu onCloseWeaponsMenu;

    public bool UIMoving { get; private set; } = false;
    public bool WeaponMenuOpen { get; private set; } = false;
    public bool WeaponsActive { get; private set; } = false;
    public bool HasFiredWeapon { get; private set; } = false;
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
            fireHeld = true;
            tempWeaponInfo = weaponInfo;
            tempPlayerPosition = playerPosition;
            cameraSystem.ResetCamera();
            turnManager.CurrentPlayerTurn.ResetFreeCamSetting();

            Ray ray = new Ray(aimArrow.anchoredPosition, aimArrow.up * 5);
            aimPosition = ray.GetPoint(5.0f);
        }
        else if (fireHeld == true) {
            StartCoroutine(FireWeaponCoroutine(weaponInfo, playerPosition));
        }
    }

    private IEnumerator FireWeaponCoroutine(BaseWeaponSO weaponInfo, Transform playerPosition) {
        fireHeld = false;
        tempWeaponInfo = null;
        tempPlayerPosition = null;

        if (weaponInfo.isMultiShot == false) {
            OnShoot(weaponInfo, playerPosition);
        }
        else if (weaponInfo.isMultiShot == true) { 
            for (int i = 0; i < weaponInfo.numOfShots; i++) {
                OnShoot(weaponInfo, playerPosition);
                yield return new WaitForSeconds(weaponInfo.delayBetweenShots);
            }
        }

        WaitTillWeaponsFinished();
    }

    private void OnShoot(BaseWeaponSO weaponInfo, Transform playerPosition) {
        Vector3 spawnPos = new Vector3(playerPosition.position.x, playerPosition.position.y + 1, 0);
        GameObject newWeapon = Instantiate(weaponInfo.weaponPrefab, spawnPos, Quaternion.identity);
        Rigidbody rb = newWeapon.GetComponent<Rigidbody>();
        WeaponScript weaponScript = newWeapon.GetComponent<WeaponScript>();
        Vector3 aimPos = new Vector3(aimPosition.x, aimPosition.y, 0);

        if (weaponInfo.weaponRandomisation == true) {
            aimPos.y *= Random.Range(weaponInfo.minimumRandomness, weaponInfo.maximumRandomness);
        }

        //Calculate velocity
        Vector3 weaponDirection = aimPos - playerPosition.position;
        Vector2 weaponVelocity = new Vector2(weaponDirection.x, weaponDirection.y).normalized * weaponInfo.weaponSpeed * aimStrength * (weaponInfo.weaponRandomisation == true ? Random.Range(weaponInfo.minimumRandomness, weaponInfo.maximumRandomness) : 1.0f);
        rb.velocity = weaponVelocity;

        //Set rotation
        if (weaponInfo.useAutomaticRotation == true) {
            Vector3 weaponRotation = playerPosition.position - aimPos;
            float originalRotation = Mathf.Atan2(weaponRotation.y, weaponRotation.x) * Mathf.Rad2Deg;
            newWeapon.transform.rotation = Quaternion.Euler(0, 0, originalRotation);
        }

        //Set weapon values
        rb.useGravity = weaponInfo.useGravity;
        weaponScript.SetupWeapon(weaponInfo, playerPosition.GetComponent<Collider>());
        activeWeapons.Add(newWeapon);
        WeaponsActive = true;
        HasFiredWeapon = true;
        turnManager.CurrentAntTurn.SetCanMove(!HasFiredWeapon);
        cameraSystem.ResetCamera();
        turnManager.CurrentPlayerTurn.ResetFreeCamSetting();
        cameraSystem.SetCameraTarget(newWeapon.transform);
        cameraSystem.SetCameraZoomingBool(false);

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
            HasFiredWeapon = true;
            turnManager.CurrentAntTurn.SetCanMove(!HasFiredWeapon);

            Ray ray = new Ray(aimArrow.anchoredPosition, aimArrow.up * 5);
            aimPosition = ray.GetPoint(2.5f);

            Collider[] colliders = Physics.OverlapSphere(aimPosition, 2.5f).Where(x => x.CompareTag("Player") && x.gameObject != turnManager.CurrentAntTurn.gameObject).ToArray();

            if (colliders.Length > 0) {
                colliders.First().GetComponent<Ant>().TakeDamage(weaponInfo.baseDamage);
                colliders.First().GetComponent<Ant>().UnFreezeMovement();
                colliders.First().GetComponent<Rigidbody>().AddExplosionForce(weaponInfo.knockbackStrength, playerPosition.position, 0, weaponInfo.upwardsModifier, ForceMode.Impulse);

                if (weaponInfo.weaponEffect != null) {
                    weaponInfo.weaponEffect.GetComponent<EffectScript>().AddEffect(colliders.First().GetComponent<Ant>());
                }
            }

            if (weaponInfo.cameraShakeOnFire) {
                cameraSystem.StartCameraShake(weaponInfo.cameraShakeDuration, weaponInfo.cameraShakeIntensity);
            }

            cameraSystem.CameraDelay(weaponInfo.cameraDelay);
            cameraSystem.ResetCamera();
            turnManager.CurrentPlayerTurn.ResetFreeCamSetting();
            cameraSystem.SetCameraTarget(playerPosition.position);
            cameraSystem.SetCameraZoomingBool(false);
            WaitTillWeaponsFinished();
        }
    }

    public void UseSprayWeapon(BaseWeaponSO weaponInfo, Transform playerPosition) {
        if (canFireWeapon) {
            canFireWeapon = false;
            canAim = false;

            Ray ray = new Ray(aimArrow.anchoredPosition, aimArrow.up * 5);
            aimPosition = ray.GetPoint(2.5f);

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

            sprayArea.GetComponent<SprayAreaScript>().Setup(weaponInfo, turnManager);

            if (weaponInfo.cameraShakeOnFire) {
                cameraSystem.StartCameraShake(weaponInfo.cameraShakeDuration, weaponInfo.cameraShakeIntensity);
            }

            activeWeapons.Add(sprayArea);
            WeaponsActive = true;
            HasFiredWeapon = true;
            turnManager.CurrentAntTurn.SetCanMove(!HasFiredWeapon);
            cameraSystem.ResetCamera();
            turnManager.CurrentPlayerTurn.ResetFreeCamSetting();
            cameraSystem.CameraDelay(weaponInfo.cameraDelay);
            cameraSystem.SetCameraTarget(playerPosition.position);
            cameraSystem.SetCameraZoomingBool(false);
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
        RemoveWorldWeapon();

        if (WeaponSelected != null && WeaponSelected.unlimitedUses == false) {
            turnManager.CurrentPlayerTurn.RemoveWeapon(WeaponSelected);
        }

        turnManager.EndTurn();
    }

    //Handles setting aimValue
    public void AimWeapon(InputValue inputValue) {
        aimValue = inputValue.Get<Vector2>();
    }

    private void ArrowAim() {
        if (aimValue.x < -0.25f) {
            aimPosition.x = turnManager.CurrentAntTurn.transform.position.x - 5;
        }
        else if (aimValue.x > 0.25f) {
            aimPosition.x = turnManager.CurrentAntTurn.transform.position.x + 5;
        }

        float zRot = aimValue.y * aimRotationSpeed * Time.deltaTime;

        if (aimingLeft == false) {
            aimRot = aimArrow.rotation.eulerAngles.z - zRot;
        }
        else {
            aimRot = aimArrow.rotation.eulerAngles.z + zRot;
        }

        if (aimPosition.x < turnManager.CurrentAntTurn.transform.position.x) {
            Vector3 arrowPos = turnManager.CurrentAntTurn.transform.position;
            arrowPos.x -= 1.5f;
            aimArrow.anchoredPosition = arrowPos;
            turnManager.CurrentAntTurn.RotateAnt(Vector2.left);

            if (aimingLeft == false) {
                aimingLeft = true;
                aimArrow.rotation = Quaternion.Euler(0, 0, 90);
            }
            else if (aimRot <= 178 && aimRot >= 2) {
                aimArrow.rotation = Quaternion.Euler(0, 0, aimArrow.rotation.eulerAngles.z - zRot);
            }
            else if (aimRot > 178 && aimRot < 180) {
                aimArrow.rotation = Quaternion.Euler(0, 0, 177);
            }
            else if (aimRot < 2 && aimRot > 0) {
                aimArrow.rotation = Quaternion.Euler(0, 0, 3);
            }
        }
        else {
            Vector3 arrowPos = turnManager.CurrentAntTurn.transform.position;
            arrowPos.x += 1.5f;
            aimArrow.anchoredPosition = arrowPos;
            turnManager.CurrentAntTurn.RotateAnt(Vector2.right);

            if (aimingLeft == true) {
                aimingLeft = false;
                aimArrow.rotation = Quaternion.Euler(0, 0, -90);
            }
            else if (aimRot <= 358 && aimRot >= 182) {
                aimArrow.rotation = Quaternion.Euler(0, 0, aimArrow.rotation.eulerAngles.z + zRot);
            }
            else if (aimRot > 358 && aimRot < 360) {
                aimArrow.rotation = Quaternion.Euler(0, 0, -3);
            }
            else if (aimRot < 182 && aimRot > 180) {
                aimArrow.rotation = Quaternion.Euler(0, 0, -177);
            }
        }
    }

    private void ArrowStrength() {
        aimStrength += 0.5f * Time.deltaTime;

        if (aimStrength >= 2.0f) {
            fireHeld = false;
            aimStrength = 2.0f;
            StartCoroutine(FireWeaponCoroutine(tempWeaponInfo, tempPlayerPosition));
        }
    }

    private void Update() {
        if (WeaponSelected != null && canAim == true) {
            ArrowAim();
        }

        if (fireHeld == true) {
            UpdateArrowSize();
            ArrowStrength();
        }
    }

    //Resets the aim to a default position
    private void ResetAimPosition() {
        aimPosition = turnManager.CurrentAntTurn.transform.position;
        aimPosition.x += 2;

        aimArrow.gameObject.SetActive(true);
        aimArrow.rotation = Quaternion.Euler(0, 0, -90);
        aimRot = aimArrow.rotation.eulerAngles.z;
        canAim = true;

        Vector3 arrowPos = turnManager.CurrentAntTurn.transform.position;
        arrowPos.x += 1.5f;
        aimArrow.anchoredPosition = arrowPos;
        aimArrow.localScale = new Vector3(1, 1, 1);
        aimStrength = 0.1f;

        turnManager.CurrentAntTurn.RotateAnt(Vector2.right);
    }

    private void UpdateArrowSize() {
        float arrowSize = aimArrowDefaultSize * aimStrength;
        aimArrow.transform.localScale = new Vector3(arrowSize, arrowSize, arrowSize);
    }

    //Used to either open or close the weapon menu
    public void WeaponMenu() {
        if (WeaponMenuOpen == false && UIMoving == false && WeaponSelected != null) {
            WeaponSelected = null;
            aimArrow.gameObject.SetActive(false);
            onCloseWeaponsMenu?.Invoke();
            RemoveWorldWeapon();
            turnManager.CurrentAntTurn.ResetAnimation();
            turnManager.FireWeaponText.GetComponent<FadeScript>().FadeOutUI(1.0f);
            turnManager.WeaponMenuText.GetComponent<FadeScript>().FadeInUI(1.0f);
        }
        else if (WeaponMenuOpen == true && UIMoving == false) {
            UIMoving = true;
            StartCoroutine(CloseWeaponMenuCoroutine(true));
        }
        else if (WeaponMenuOpen == false && UIMoving == false && HasFiredWeapon == false) {
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
        HasFiredWeapon = false;

        if (turnManager.CurrentAntTurn != null) {
            turnManager.CurrentAntTurn.SetCanMove(!HasFiredWeapon);
        }

        turnManager.WeaponMenuText.GetComponent<FadeScript>().FadeInUI(1.0f);
        turnManager.FireWeaponText.GetComponent<FadeScript>().FadeOutUI(1.0f);

        StartCoroutine(ForceCloseWeaponMenuCoroutine());
    }

    //Called at the end of a turn. Forces the weapon menu to close.
    private IEnumerator ForceCloseWeaponMenuCoroutine() {
        if (UIMoving == true) {
            yield return new WaitUntil(() => UIMoving == false);
        }

        if (WeaponMenuOpen == true) {
            StartCoroutine(CloseWeaponMenuCoroutine(true));
        }
    }

    //Closes the queen ant health ui and opens the weapons menu
    private IEnumerator OpenWeaponMenuCoroutine() {
        onOpenWeaponsMenu?.Invoke();
        CheckAllIcons();
        weaponMenuUI.SetActive(true);
        turnManager.CurrentPlayerTurn.GetEventSystem().SetSelectedGameObject(weaponIcons.Where(x => x.Interactable == true).First().GetButton());
        weaponMenuUI.GetComponent<MoveUI>().StartMoveUI(LerpType.OutBack, weaponMenuUI, new Vector2(-750, 50), new Vector2(50, 50), 1.0f);
        turnManager.WeaponMenuText.GetComponent<FadeScript>().FadeOutUI(1.0f);

        yield return new WaitUntil(() => weaponMenuUI.GetComponent<RectTransform>().anchoredPosition == new Vector2(50, 50));
        EnableIconInteraction();
        UIMoving = false;
    }

    //Closes the weapons menu and opens the queen ant health UI
    private IEnumerator CloseWeaponMenuCoroutine(bool invokeCloseWeaponsMenu) {
        DisableIconInteraction();
        weaponMenuUI.GetComponent<MoveUI>().StartMoveUI(LerpType.InBack, weaponMenuUI, new Vector2(50, 50), new Vector2(-750, 50), 1.0f);

        yield return new WaitUntil(() => weaponMenuUI.GetComponent<RectTransform>().anchoredPosition == new Vector2(-750, 50));
        weaponMenuUI.SetActive(false);
        WeaponMenuOpen = false;

        if (invokeCloseWeaponsMenu) {
            onCloseWeaponsMenu?.Invoke();
            turnManager.WeaponMenuText.GetComponent<FadeScript>().FadeInUI(1.0f);
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

        string knockbackLevel = weapon.knockbackLevel.ToString();

        if (knockbackLevel == "Medium") {
            knockbackLevel = "Med.";
        }

        weaponDescriptionText.text = "Type: " + (weapon.isMelee ? "Melee" : "Ranged") + "\nDamage: " + weapon.baseDamage + "\nKnockback: " + knockbackLevel + "\n\n" + weapon.weaponDescription;
    }

    public void SetSelectedWeapon(BaseWeaponSO weapon) {
        WeaponSelected = weapon;

        if (weapon.worldWeaponPrefab != null) {
            GameObject worldWeapon = Instantiate(weapon.worldWeaponPrefab, turnManager.CurrentAntTurn.WeaponTransform);
            worldWeapon.transform.localScale *= weapon.worldWeaponScale;
            worldWeapon.transform.localPosition = weapon.worldWeaponLocalPos;
            worldWeapon.transform.localEulerAngles = weapon.worldWeaponRotation;

            turnManager.CurrentAntTurn.ResetAnimation();
            turnManager.CurrentAntTurn.ChangeAnimation("UsingBackWeapon");
        }

        turnManager.FireWeaponText.GetComponent<FadeScript>().FadeInUI(1.0f);
        ResetAimPosition();
        StartCoroutine(CloseWeaponMenuCoroutine(false));
    }

    private void RemoveWorldWeapon() {
        if (turnManager.CurrentAntTurn != null && turnManager.CurrentAntTurn.WeaponTransform.childCount > 0 && turnManager.CurrentAntTurn.WeaponTransform.GetChild(0) != null) {
            Destroy(turnManager.CurrentAntTurn.WeaponTransform.GetChild(0).gameObject);
        }    
    }
}