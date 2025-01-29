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
    private VirtualMouseInput mouseInput;
    private Plane plane = new Plane(Vector3.forward, 0);
    private List<BaseWeaponSO> allWeapons = new List<BaseWeaponSO>();

    private void Start() { 
        mouseInput = FindFirstObjectByType<VirtualMouseInput>();
        allWeapons = Resources.LoadAll<BaseWeaponSO>("").ToList();
    }

    public void FireWeapon(BaseWeaponSO weaponInfo, Transform playerPosition) {
        GameObject newWeapon = Instantiate(weaponInfo.weaponPrefab, playerPosition.position, Quaternion.identity);
        Rigidbody rb = newWeapon.GetComponent<Rigidbody>();
        WeaponScript weaponScript = newWeapon.GetComponent<WeaponScript>();

        //Calculate virtual mouse position in world space
        float distance;
        Vector3 mousePosInWorldSpace = Vector3.one;
        Ray ray = Camera.main.ScreenPointToRay(mouseInput.cursorTransform.position);

        if (plane.Raycast(ray, out distance)) {
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
        //Queen ant health UI needs to be hidden. Once added.

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

        //Queen ant health UI needs to be shown. Once added.

        uiMoving = false;
    }
}