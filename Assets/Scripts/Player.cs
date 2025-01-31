using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour {
    [Header("Player Info")]
    public PlayerInfo playerInfo = new PlayerInfo();

    [Header("Game Info")]
    [SerializeField] private BaseWeaponSO currentWeapon;

    private LobbyManager lobbyManager;
    private TurnManager turnManager;
    private WeaponManager weaponManager;

    private List<GameObject> antList = new List<GameObject>();
    private GameObject queenAnt;
    private List<BaseWeaponSO> currentWeapons = new List<BaseWeaponSO>();

    private void Awake() {
        lobbyManager = FindFirstObjectByType<LobbyManager>();
        SceneManager.activeSceneChanged += ActiveSceneChanged;

        DontDestroyOnLoad(gameObject);
    }

    private void ActiveSceneChanged(Scene currentScene, Scene nextScene) {
        if (nextScene.name.Contains("Game")) {
            turnManager = FindFirstObjectByType<TurnManager>();
            weaponManager = FindFirstObjectByType<WeaponManager>();
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
        if (turnManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) {
            turnManager.EndTurn();
        }
    }

    //Function called when the player presses the fire weapon button
    private void OnFireWeapon() {
        if (turnManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) {
            weaponManager.FireWeapon(currentWeapon, transform);
        }
    }

    //Function called when the player presses the move button
    private void OnMove(InputValue value) {
        if (turnManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) {
            if (turnManager.CurrentAntTurn != null) {
                antList.Where(x => turnManager.CurrentAntTurn == x.GetComponent<AntScript>()).First().GetComponent<AntScript>().OnMove(value); //Move for normal ants
            } else {
                queenAnt.GetComponent<QueenAntScript>().OnMove(value); //Move for queen ants
            }
        }
    }

    //Function called when the player presses the jump button
    private void OnJump() {
        if (turnManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) {
            if (turnManager.CurrentAntTurn != null) {
                antList.Where(x => turnManager.CurrentAntTurn == x.GetComponent<AntScript>()).First().GetComponent<AntScript>().OnJump(); //Jump for normal ants
            } else {
                queenAnt.GetComponent<QueenAntScript>().OnJump(); //Jump for queen ants
            }
        }
    }

    //Function called when the player presses the open weapon menu button
    private void OnWeaponMenu() {
        if (turnManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) {
            weaponManager.WeaponMenu();
        }
    }

    public AntScript GetAnt(AntScript currentAnt) {
        if (currentAnt != null) {
            currentAnt.moveVector = Vector3.zero;
        }

        List<AntScript> possibleAnts = new List<AntScript>();
        for (int i = 0; i < antList.Count; i++) {
            AntScript nextAnt = antList[i].GetComponent<AntScript>();
            if (nextAnt.hasHadTurn == false) {
                possibleAnts.Add(nextAnt);
            }
        }

        if (possibleAnts.Count > 0) {
            int nextAntIndex = Random.Range(0, possibleAnts.Count);
            return possibleAnts[nextAntIndex];
        } else {
            return null;
        }
    }

    public void ResetAnts() {
        for (int i = 0; i < antList.Count; i++) {
            antList[i].GetComponent<AntScript>().hasHadTurn = false;
            Debug.Log("ant turn again");
        }
    }

    public void ResetQueen() {
        queenAnt.GetComponent<QueenAntScript>().moveVector = Vector3.zero;

    }
}