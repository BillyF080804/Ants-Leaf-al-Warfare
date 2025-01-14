using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class WeaponManager : MonoBehaviour {
    private VirtualMouseInput mouseInput;
    Plane plane = new Plane(Vector3.forward, 0);

    private void Start() { 
        mouseInput = FindFirstObjectByType<VirtualMouseInput>();
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

    //Code to ensure the aiming reticle stays inside the screen
    //Will get changed to a different way of doing this. I hope.
    private void LateUpdate() {
        Vector2 virtualMousePos = mouseInput.virtualMouse.position.value;
        virtualMousePos.x = Mathf.Clamp(virtualMousePos.x, 0f, Screen.width);
        virtualMousePos.y = Mathf.Clamp(virtualMousePos.y, 0f, Screen.height);
        InputState.Change(mouseInput.virtualMouse.position, virtualMousePos);
    }
}