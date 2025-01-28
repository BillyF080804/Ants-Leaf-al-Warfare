using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
    [Header("Player Info")]
    public PlayerInfo playerInfo = new PlayerInfo();

    [Header("Game Info")]
    [SerializeField] private BaseWeaponSO currentWeapon;
    
    private PlayerManager playerManager;
    private TurnManager turnManager;
    private WeaponManager weaponManager;

    private List<GameObject> antList = new List<GameObject>();
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

    private void OnMove() {
        if (turnManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) {
            
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
		Debug.Log("hi");
		for (int i = 0;i < antList.Count;i++) {
            antList[i].GetComponent<AntScript>().hasHadTurn = false;
        }
    }
}