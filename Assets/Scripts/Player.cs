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
}