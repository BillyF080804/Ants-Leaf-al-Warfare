using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour {
    [Header("Player Info")]
    public PlayerInfo playerInfo = new PlayerInfo();

    private Coroutine skipTurnCoroutine;
    private CameraSystem cameraSystem;
    private LobbyManager lobbyManager;
    private TurnManager turnManager;
    private WeaponManager weaponManager;

    public bool ConfirmedQueenSpawn { get; private set; } = false;
    private bool canSpawnQueen = false;

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
    }

    public void RemoveQueen() {
        QueenAnt = null;
    }

    //Function called when the player presses the change color button
    private void OnChangeColor() {
        if (lobbyManager != null && SceneManager.GetActiveScene().name.Contains("Menu")) {
            lobbyManager.ChangeColor(playerInfo.playerNum); //Only works in the menu
        }
    }

    //Function called when the player presses the skip turn button
    private void OnSkipTurn() {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false) {
            if (skipTurnCoroutine == null) {
                skipTurnCoroutine = StartCoroutine(SkipTurnCoroutine(2.0f));
                cameraSystem.ZoomCameraIn(2.0f);
            }
            else if (skipTurnCoroutine != null) {
                if (cameraSystem.IsZoomingOut == false) {
                    cameraSystem.ZoomCameraOut(0.5f);
                }

                StopCoroutine(skipTurnCoroutine);
                skipTurnCoroutine = null;
            }
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
    }

    //Function called when the player presses the move button
    private void OnMove(InputValue value) {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected == null) {
            if (turnManager.CurrentAntTurn != null) {
                if(turnManager.CurrentAntTurn.GetComponent<QueenAntScript>() != null) {
                    QueenAnt.GetComponent<Ant>().OnMove(value);
                } else {
					AntList.Where(x => turnManager.CurrentAntTurn == x.GetComponent<Ant>()).First().GetComponent<Ant>().OnMove(value); //Move for normal ants

				}
			} 
        }
    }

    //Function called when the player presses the jump button
    private void OnJump() {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected == null) {
            if (turnManager.CurrentAntTurn != null) {
                if (turnManager.CurrentAntTurn.GetComponent<QueenAntScript>() != null) {
                    QueenAnt.GetComponent<Ant>().OnJump();
                } else {
                    AntList.Where(x => turnManager.CurrentAntTurn == x.GetComponent<Ant>()).First().GetComponent<Ant>().OnJump(); //Jump for normal ants
                }
            }
        }
    }

    //Function called when the player presses the open weapon menu button
    private void OnWeaponMenu() {
        if (CheckActionIsValid()) {
            weaponManager.WeaponMenu();
        }
    }

    //Function called when player has weapon selected and is aiming
    private void OnAim(InputValue value) {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected != null) {
            weaponManager.AimWeapon(value);
        }
    }

    //Function called when player has weapon selected and changes strength of shot
    private void OnAimStrength(InputValue value) {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected != null) {
            weaponManager.AimStrength(value);
        }
    }

    //Function called when selecting where to spawn queen
    private void OnSpawnQueenAnt() {
        if (canSpawnQueen == true) {
            canSpawnQueen = false;
            ConfirmedQueenSpawn = true;
        }
    }

    private bool CheckActionIsValid() {
        if (turnManager != null && weaponManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) {
            return true;
        }
        else {
            return false;
        }
    }

    public void StopSkipTurn() {
        if (skipTurnCoroutine != null) {
            StopCoroutine(skipTurnCoroutine);
        }    
    }

    private IEnumerator SkipTurnCoroutine(float skipDuration) {
        float timeElapsed = 0.0f;

        while (timeElapsed < skipDuration) {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        cameraSystem.ZoomCameraOut(0.5f);
        turnManager.EndTurn();
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
        } else {
            Ant testAnt = QueenAnt.GetComponent<Ant>();

			if (testAnt.hasHadTurn) {
				return null;
            } else {
                return testAnt;

			}
            
        }
    }

    public void ResetAnts() {
        for (int i = 0; i < AntList.Count; i++) {
            AntList[i].GetComponent<Ant>().hasHadTurn = false;
        }
		QueenAnt.GetComponent<Ant>().hasHadTurn = false;
	}


    public void AddNewWeapon(BaseWeaponSO newWeapon) {
        CurrentWeapons.Add(newWeapon);
    }


    public void RemoveWeapon(BaseWeaponSO weapon) {
        CurrentWeapons.Remove(weapon);

    }

    public void AllowPlayerToSpawnQueen() {
        canSpawnQueen = true;
    }

    //Temporary Func/Keybind of Left Shift
    private void OnQueenAttack() {
        if (CheckActionIsValid()) {
			QueenAnt.GetComponent<QueenAntScript>().SpecialAttack();
        }
    }


    private void OnInteract() {
        if (CheckActionIsValid() && turnManager.CurrentAntTurn.canInteract) { 
            Debug.Log("ButtonPress");
            turnManager.CurrentAntTurn.interactable.Interaction();
        }
    }
}