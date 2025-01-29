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
    
    private PlayerManager playerManager;
    private TurnManager turnManager;
    private WeaponManager weaponManager;

    private List<GameObject> antList = new List<GameObject>();
    private GameObject queenAnt;
    private List<BaseWeaponSO> currentWeapons = new List<BaseWeaponSO>();

    private void Awake() {
        playerManager = FindFirstObjectByType<PlayerManager>();    
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

    private void OnSkipTurn() {
        if (turnManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) {
            Debug.Log(playerInfo.playerNum + " skipping turn");
            turnManager.EndTurn();
        }
    }

    private void OnChangeColor() {
        if (playerManager != null && SceneManager.GetActiveScene().name.Contains("Menu")) {
            playerManager.ChangeColor(playerInfo.playerNum);
        }
    }

    private void OnFireWeapon() {
        if (turnManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) {
            weaponManager.FireWeapon(currentWeapon, transform);
        }
    }

    private void OnMove(InputValue value) {
        if (turnManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) { 
            if (turnManager.CurrentAntTurn != null) {
                antList.Where(x => turnManager.CurrentAntTurn == x.GetComponent<AntScript>()).First().GetComponent<AntScript>().OnMove(value);
            }
            else {
                queenAnt.GetComponent<QueenAntScript>().OnMove(value);
            }
        }
    }

    private void OnJump() {
        if (turnManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) {
            if (turnManager.CurrentAntTurn != null) {
                antList.Where(x => turnManager.CurrentAntTurn == x.GetComponent<AntScript>()).First().GetComponent<AntScript>().OnJump();
            }
            else {
                queenAnt.GetComponent<QueenAntScript>().OnJump();
            }
        }
    }

    public AntScript GetAnt(AntScript currentAnt) {
        if(currentAnt != null) {
			currentAnt.moveVector = Vector3.zero;
		}
       

        List<AntScript> possibleAnts = new List<AntScript>();
        for(int i = 0; i < antList.Count; i++) {
            AntScript nextAnt = antList[i].GetComponent<AntScript>();
			if (nextAnt.hasHadTurn == false) {
                possibleAnts.Add(nextAnt);
            }
        }

        if(possibleAnts.Count > 0) {
			int nextAntIndex = Random.Range(0, possibleAnts.Count);
			return possibleAnts[nextAntIndex];
		}
        else { 
            return null; 
        }
    }

    public void ResetAnts() {
		for (int i = 0;i < antList.Count;i++) {
            antList[i].GetComponent<AntScript>().hasHadTurn = false;
        }
    }
}