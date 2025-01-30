using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class WeaponManager : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private GameObject weaponMenuUI;

    public bool WeaponMenuOpen { get; private set; } = false;
    private bool uiMoving = false;

    private Plane plane = new Plane(Vector3.forward, 0);
    private TurnManager turnManager;
    private VirtualMouseInput mouseInput;
    private List<BaseWeaponSO> allWeapons = new List<BaseWeaponSO>();

    private void Start() { 
        turnManager = FindFirstObjectByType<TurnManager>();
        mouseInput = FindFirstObjectByType<VirtualMouseInput>();
        allWeapons = Resources.LoadAll<BaseWeaponSO>("").ToList();
    }

    public void FireWeapon(BaseWeaponSO weaponInfo, Transform playerPosition) {
        GameObject newWeapon = Instantiate(weaponInfo.weaponPrefab, playerPosition.position, Quaternion.identity);
        Rigidbody rb = newWeapon.GetComponent<Rigidbody>();
        WeaponScript weaponScript = newWeapon.GetComponent<WeaponScript>();

        //Calculate virtual mouse position in world space
        Vector3 mousePosInWorldSpace = Vector3.one;
        Ray ray = Camera.main.ScreenPointToRay(mouseInput.cursorTransform.position);

        if (plane.Raycast(ray, out float distance)) {
            mousePosInWorldSpace = ray.GetPoint(distance);
        }

        //Calculate velocity & rotation
        Vector3 weaponDirection = mousePosInWorldSpace - playerPosition.position;
        Vector3 weaponRotation = playerPosition.position - mousePosInWorldSpace;

        //Set velocity
        Vector2 weaponVelocity = new Vector2(weaponDirection.x, weaponDirection.y).normalized * weaponInfo.weaponSpeed;
        rb.velocity = weaponVelocity;

        //Set rotation
        float originalRotation = Mathf.Atan2(weaponRotation.y, weaponRotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, originalRotation);

        //Set weapon values
        rb.useGravity = weaponInfo.useGravity;
        weaponScript.weaponInfo = weaponInfo;
        
        if (weaponInfo.explosive && !weaponInfo.explodeOnImpact) {
            weaponScript.StartFuse();
        }
    }

    //Code to ensure the aiming reticle stays inside the screen.
    //Will get changed to a different way of doing this. I hope.
    private void LateUpdate() {
        Vector2 virtualMousePos = mouseInput.virtualMouse.position.value;
        virtualMousePos.x = Mathf.Clamp(virtualMousePos.x, 0f, Screen.width);
        virtualMousePos.y = Mathf.Clamp(virtualMousePos.y, 0f, Screen.height);
        InputState.Change(mouseInput.virtualMouse.position, virtualMousePos);
    }

    public void WeaponMenu() {
        if (WeaponMenuOpen == true && uiMoving == false) {
            uiMoving = true;
            StartCoroutine(CloseWeaponMenuCoroutine());
        }
        else if (WeaponMenuOpen == false && uiMoving == false) {
            uiMoving = true;
            StartCoroutine(OpenWeaponMenuCoroutine());
        }
    }

    public void ForceCloseWeaponMenu() {
        StartCoroutine(ForceCloseWeaponMenuCoroutine());
    }

    private IEnumerator ForceCloseWeaponMenuCoroutine() {
        if (uiMoving == true) {
            yield return new WaitUntil(() => uiMoving == false);
        }

        if (WeaponMenuOpen == true) {
            StartCoroutine(CloseWeaponMenuCoroutine());
        }
    }

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

        WeaponMenuOpen = true;
        weaponMenuUI.SetActive(true);
        weaponMenuUI.GetComponent<MoveUI>().StartMoveUI(LerpType.OutBack, weaponMenuUI, new Vector2(-500, 50), new Vector2(50, 50), 1.0f);

        yield return new WaitUntil(() => weaponMenuUI.GetComponent<RectTransform>().anchoredPosition == new Vector2(50, 50));
        uiMoving = false;
    }

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
}