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

    private GameObject queenAnt;
    private List<GameObject> antList = new List<GameObject>();
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
        antList.Add(newAnt);
    }

    public void AddQueen(GameObject newQueen) {
        queenAnt = newQueen;
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
                    queenAnt.GetComponent<Ant>().OnMove(value);
                } else {
					antList.Where(x => turnManager.CurrentAntTurn == x.GetComponent<Ant>()).First().GetComponent<Ant>().OnMove(value); //Move for normal ants

				}
			} 
        }
    }

    //Function called when the player presses the jump button
    private void OnJump() {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected == null) {
            if (turnManager.CurrentAntTurn != null) {
                if (turnManager.CurrentAntTurn.GetComponent<QueenAntScript>() != null) {
                    queenAnt.GetComponent<Ant>().OnJump();
                } else {
                    antList.Where(x => turnManager.CurrentAntTurn == x.GetComponent<Ant>()).First().GetComponent<Ant>().OnJump(); //Jump for normal ants
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

    private bool CheckActionIsValid() {
        if (turnManager != null && weaponManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) {
            return true;
        }
        else {
            return false;
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
        for (int i = 0; i < antList.Count; i++) {
			Ant nextAnt = antList[i].GetComponent<Ant>();
            if (nextAnt.hasHadTurn == false) {
                possibleAnts.Add(nextAnt);
            }
        }

        if (possibleAnts.Count > 0) {
            int nextAntIndex = Random.Range(0, possibleAnts.Count);
            return possibleAnts[nextAntIndex];
        } else {
            Ant testAnt = queenAnt.GetComponent<Ant>();

			if (testAnt.hasHadTurn) {
				return null;
            } else {
                return testAnt;

			}
            
        }
    }

    public void ResetAnts() {
        for (int i = 0; i < antList.Count; i++) {
            antList[i].GetComponent<Ant>().hasHadTurn = false;
        }
		queenAnt.GetComponent<Ant>().hasHadTurn = false;
	}


    public void AddNewWeapon(BaseWeaponSO newWeapon) {
        CurrentWeapons.Add(newWeapon);
    }


    public void RemoveWeapon(BaseWeaponSO weapon) {
        CurrentWeapons.Remove(weapon);

    }
    //Temporary Func/Keybind of Left Shift
    private void OnQueenAttack() {
        if (turnManager.CurrentAntTurn == queenAnt.GetComponent<Ant>()) {
			queenAnt.GetComponent<QueenAntScript>().SpecialAttack();
        }
    }
}