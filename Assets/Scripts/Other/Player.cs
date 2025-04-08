using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour {
    [Header("Player Info")]
    public PlayerInfo playerInfo = new PlayerInfo();

    [Header("Settings")]
    [SerializeField] private List<TMP_SpriteAsset> uiSprites = new List<TMP_SpriteAsset>();

    private CameraSystem cameraSystem;
    private LobbyManager lobbyManager;
    private TurnManager turnManager;
    private WeaponManager weaponManager;
    private QueenBaseAntScript queenBaseAntScript;

    private bool freeCamEnabled = false;

    public GameObject QueenAnt { get; private set; } = null;
    public List<GameObject> AntList { get; private set; } = new List<GameObject>();
    public List<BaseWeaponSO> CurrentWeapons { get; private set; } = new List<BaseWeaponSO>();

    private void Awake() {
        lobbyManager = FindFirstObjectByType<LobbyManager>();
        SceneManager.activeSceneChanged += ActiveSceneChanged;

        DontDestroyOnLoad(gameObject);
    }

    private void ActiveSceneChanged(Scene currentScene, Scene nextScene) {
        if (nextScene.name.Contains("Game")) {
            turnManager = FindFirstObjectByType<TurnManager>();
            weaponManager = FindFirstObjectByType<WeaponManager>();
            cameraSystem = FindFirstObjectByType<CameraSystem>();
        }
    }

    public void AddNewAnt(GameObject newAnt) {
        AntList.Add(newAnt);
    }

    public void RemoveAnt(GameObject ant) {
        AntList.Remove(ant);
    }

    public void AddQueen(GameObject newQueen) {
        QueenAnt = newQueen;
        queenBaseAntScript = QueenAnt.GetComponent<QueenBaseAntScript>();
    }

    public void RemoveQueen() {
        QueenAnt = null;
    }

    //Called when the player presses the ready up button
    private void OnReadyUp() {
        if (lobbyManager != null && SceneManager.GetActiveScene().name.Contains("Lobby")) {
            lobbyManager.ReadyUp(playerInfo.playerNum); //Only works in the lobby
        }
    }

    //Called when the player presses a change queen specialism button
    private void OnChangeQueenSpecialism(InputValue value) {
        if (lobbyManager != null && SceneManager.GetActiveScene().name.Contains("Lobby")) {
            lobbyManager.ChangeQueenSpecialism(playerInfo.playerNum, value.Get<float>()); //Only works in the lobby
        }
    }

    //Skip turn function
    private void OnSkipTurn() {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponsActive == false) {
            turnManager.EndTurn();
        }
    }

    //Function called when the player presses the fire weapon button
    private void OnFireWeapon() {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected != null) {
            if (weaponManager.WeaponSelected.isMelee) {
                weaponManager.UseMeleeWeapon(weaponManager.WeaponSelected, turnManager.CurrentAntTurn.transform);
            }
            else if (weaponManager.WeaponSelected.isSpray) {
                weaponManager.UseSprayWeapon(weaponManager.WeaponSelected, turnManager.CurrentAntTurn.transform);
            }
            else {
                weaponManager.FireWeapon(weaponManager.WeaponSelected, turnManager.CurrentAntTurn.transform);
            }
        } 
        else if (CheckActionIsValid() && turnManager.CurrentAntTurn.GetComponent<MummyScript>() != null) {
            turnManager.CurrentAntTurn.GetComponent<MummyScript>().Attack();
		}
    }

    //Function called when the player presses the move button
    private void OnMove(InputValue value) {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected == null) {
            if (turnManager.CurrentAntTurn != null) {
                turnManager.CurrentAntTurn.OnMove(value);
			} 
        }
    }

    //Function called when the player presses the jump button
    private void OnJump() {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected == null) {
            if (turnManager.CurrentAntTurn != null) {
                turnManager.CurrentAntTurn.OnJump();
            }
        }
    }

    //Function called when the player presses the open weapon menu button
    private void OnWeaponMenu() {
        if (CheckActionIsValid() && turnManager.CurrentAntTurn.GetComponent<MummyScript>() == null) {
            weaponManager.WeaponMenu();
        }
    }

    //Function called when player has weapon selected and is aiming
    private void OnAim(InputValue value) {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected != null) {
            weaponManager.AimWeapon(value);
        }
    }

    //Called when the player is zooming the camera in/out
    private void OnCameraZoom(InputValue value) {
        if (CheckActionIsValid()) {
            cameraSystem.SetCameraZoom(value.Get<float>());
        }
    }

    //Called when the player presses the reset camera button
    private void OnResetCameraPos() {
        if (CheckActionIsValid()) {
            freeCamEnabled = !freeCamEnabled;

            if (freeCamEnabled == false) {
                cameraSystem.ResetCamera();
            }
        }
    }

    //Called when the player moves the free camera
    private void OnMoveFreeCam(InputValue value) {
        if (CheckActionIsValid()) {
            if (freeCamEnabled) {
                cameraSystem.SetFreeCameraValue(value.Get<Vector2>());
            }
        }
    }

    private bool CheckActionIsValid() {
        if (turnManager != null && weaponManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this && turnManager.CurrentAntTurn != null) {
            return true;
        }
        else {
            return false;
        }
    }

    public Ant GetAnt(Ant currentAnt) {
        if (currentAnt != null) {
            currentAnt.moveVector = Vector3.zero;
        }

        List<Ant> possibleAnts = new List<Ant>();
        for (int i = 0; i < AntList.Count; i++) {
			Ant nextAnt = AntList[i].GetComponent<Ant>();
            if (nextAnt.hasHadTurn == false) {
                possibleAnts.Add(nextAnt);
            }
        }

        if (possibleAnts.Count > 0) {
            int nextAntIndex = Random.Range(0, possibleAnts.Count);
            return possibleAnts[nextAntIndex];
        }
        else if (QueenAnt != null) {
            Ant testAnt = QueenAnt.GetComponent<Ant>();

            if (testAnt.hasHadTurn) {
                return null;
            }
            else {
                return testAnt;
            }
        }
        else {
            Debug.LogError("Isaac pls fix ty"); 
            Debug.LogError("Mate, what's the error here? I have never seen this debug lol");
			return null;
        }
    }

    public void ResetAnts() {
        for (int i = 0; i < AntList.Count; i++) {
            AntList[i].GetComponent<Ant>().hasHadTurn = false;
        }
		QueenAnt.GetComponent<Ant>().hasHadTurn = false;
	}

    public void ResetFreeCamSetting() {
        freeCamEnabled = false;
    }

    public void AddNewWeapon(BaseWeaponSO newWeapon) {
        CurrentWeapons.Add(newWeapon);
    }

    public void RemoveWeapon(BaseWeaponSO weapon) {
        CurrentWeapons.Remove(weapon);
    }

    public int GetAllHealth() {
        int healthTotal = 0;

        for (int i = 0; i < AntList.Count; i++) {
            healthTotal += AntList[i].GetComponent<Ant>().GetHealth();
        }

        if (QueenAnt != null) {
            healthTotal += QueenAnt.GetComponent<Ant>().GetHealth();
        }

        return healthTotal;
    }

    public EventSystem GetEventSystem() {
        return GetComponent<EventSystem>();
    }

    public string GetKeybindForAction(string actionName) {
        return playerInfo.playerInput.actions.FindAction(actionName).GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
    }

    public TMP_SpriteAsset GetSpriteFromAction(string actionName) {
        switch (playerInfo.playerInput.actions.FindAction(actionName).GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions)) {
            case "A":
                return uiSprites[0];
            case "B":
                return uiSprites[1];
            case "X":
                return uiSprites[2];
            case "Y":
                return uiSprites[3];
            case "Circle":
                return uiSprites[4];
            case "Cross":
                return uiSprites[5];
            case "Square":
                return uiSprites[6];
            case "Triangle":
                return uiSprites[7];
            default:
                return null;
        }
    }

    //Temporary Func/Keybind of Left Shift
    private void OnQueenAttack() {
        Debug.Log(GetKeybindForAction("QueenAttack"));

        if (CheckActionIsValid() && turnManager.CurrentAntTurn == queenBaseAntScript) {
			queenBaseAntScript.SpecialAttack();
        }
    }


    private void OnInteract() {
        if (CheckActionIsValid() && turnManager.CurrentAntTurn.canInteract) { 
            Debug.Log("ButtonPress");
            turnManager.CurrentAntTurn.interactable.Interaction();
        }
    }



    [Header("Debug Thing: Please ignore. no")]
    [SerializeField]
    EffectScript EffectScript;
    private void OnEffectTest() {
        for(int i = 0; i < AntList.Count; i++) {
            EffectScript.AddEffect(AntList[i].GetComponent<Ant>());
            EffectScript.ApplyEffect(AntList[i].GetComponent<Ant>());

        }
		EffectScript.AddEffect(queenBaseAntScript);
		EffectScript.ApplyEffect(queenBaseAntScript);
	}
}