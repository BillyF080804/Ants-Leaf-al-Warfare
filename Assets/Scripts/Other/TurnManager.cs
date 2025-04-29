using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private string levelName;
    [SerializeField] private int maxRounds = 10;
    [SerializeField] private int maxTurnTime = 20;

    [field: Header("Ant Spawning Settings")]
    [field: SerializeField] public float MapMinX { get; private set; } = -10.0f;
    [field: SerializeField] public float MapMaxX { get; private set; } = 10.0f;
    [field: SerializeField] public float MinDistanceBetweenQueens { get; private set; } = 5.0f;

    [field: Header("Gamemode Settings")]
    [field: SerializeField] public int DamageToDealOnQueenDeath { get; private set; } = 10;

    [Header("Event Settings")]
    [SerializeField] private UnityEvent startTurnEvent;
    [SerializeField] private UnityEvent endTurnEvent;
    [SerializeField] private UnityEvent startRoundEvent;

    [Header("Queen Ant Specialisms")]
    [SerializeField] private List<AntSO> queenAntSpecials = new List<AntSO>();

    [Header("Ant Prefabs")]
    [SerializeField] private GameObject baseAntPrefab;
    [SerializeField] private GameObject queenPrefab;

    [Header("Main Game UI")]
    [SerializeField] private MoveUI mainGameUIMoveScript;
    [SerializeField] private TMP_Text turnTimeText;
    [SerializeField] private TMP_Text playerTurnText;

    [field: Header("UI")]
    [field: SerializeField] public Canvas MainCanvas { get; private set; }
    [SerializeField] private TMP_Text roundNumText;
    [SerializeField] private GameObject blackscreen;
    [SerializeField] private GameObject levelNameText;
    [SerializeField] private GameObject queenHealthUIPrefab;
    [SerializeField] private List<Sprite> queenAntHealthUIVariants;

    [field: Header("Text Hints")]
    [field: SerializeField] public FadeScript TextHintFadeScript { get; private set; }
    [field: SerializeField] public TMP_Text WeaponMenuText { get; private set; }
    [field: SerializeField] public TMP_Text FireWeaponText { get; private set; }
    [field: SerializeField] public TMP_Text QueenAttackText { get; private set; }

    private int numOfAnts = 2;
    private float currentTurnTime;
    private bool currentTurnEnded = false;
    private bool turnTimerPaused = false;
    private bool allAntsMoved = false;
    private bool gameOver = false;

    public delegate void OnTurnEnded();
    public static OnTurnEnded onTurnEnded;

    public int CurrentRound { get; private set; } = 0;
    public Player CurrentPlayerTurn { get; private set; } = null;
    public Ant CurrentAntTurn { get; private set; } = null; //Tracks which ant's turn it currently is
    public List<GameObject> QueenHealthUI { get; private set; } = new List<GameObject>();
    public List<Player> PlayerList { get; private set; } = new List<Player>();

    private Coroutine turnTimerCoroutine;
    private AntSpawner antSpawner;
    private CameraSystem cameraSystem;
    private WeaponManager weaponManager;
    private WeaponDropSystem dropSystem;
    private BBQScript bbqScript;
    private Hose hose;
    private EnterObjectManager enterObjectManager;

    private void Start() {
        numOfAnts = LoadingData.numOfAnts;
        PlayerList = LoadingData.playerList;
        dropSystem = FindFirstObjectByType<WeaponDropSystem>();
        weaponManager = FindFirstObjectByType<WeaponManager>();
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        antSpawner = FindFirstObjectByType<AntSpawner>();
        bbqScript = FindFirstObjectByType<BBQScript>();
        hose = FindFirstObjectByType<Hose>();
        enterObjectManager = FindFirstObjectByType<EnterObjectManager>();

        currentTurnTime = maxTurnTime;
        StartCoroutine(StartLevelCoroutine());
    }

    private IEnumerator StartLevelCoroutine() {
        levelNameText.GetComponent<TMP_Text>().text = levelName;
        SpawnAllAnts();

        LoadingUI loadingUI = FindFirstObjectByType<LoadingUI>();
        loadingUI.OpenShutter();
        cameraSystem.StartCameraPan();
        yield return new WaitForSeconds(1.0f);
        Destroy(loadingUI.gameObject);

        StartCoroutine(LevelTextCoroutine());
        yield return new WaitUntil(() => cameraSystem.CameraDelayActive == false);

        StartCoroutine(StartGame());
    }

    private void SpawnAllAnts() {
        for (int i = 0; i < PlayerList.Count; i++) {
            for (int j = 0; j < numOfAnts; j++) {
                GameObject newAnt = antSpawner.SpawnAnt(baseAntPrefab);
                PlayerList[i].AddNewAnt(newAnt);
                newAnt.GetComponent<Ant>().ownedPlayer = (Ant.PlayerList)i;
                newAnt.GetComponent<BaseAntScript>().ChangeAntColors(PlayerList[i].playerInfo.playerColor);
            }

            SpawnQueen(i);
        }

        enterObjectManager.OnAllAntsSpawned();
    }

    private void SpawnQueen(int playerNum) {
        GameObject newQueen = antSpawner.SpawnAnt(queenPrefab);
        newQueen.GetComponent<Ant>().ownedPlayer = (Ant.PlayerList)playerNum;

        newQueen.GetComponent<QueenBaseAntScript>().ChangeAntColors(PlayerList[playerNum].playerInfo.playerColor);
        ChangeQueenSpecialism(PlayerList[playerNum].playerInfo.queenType, newQueen.GetComponent<Ant>());
        PlayerList[playerNum].AddQueen(newQueen);
    }

    private IEnumerator LevelTextCoroutine() {
        yield return new WaitForSeconds(2.0f);
        levelNameText.GetComponent<FadeScript>().FadeOutUI(2.0f);
    }

    private IEnumerator StartGame() {
        roundNumText.text = "Current Round: 1";
        cameraSystem.SetCameraTarget(null);

        ShowRoundNumber();
        yield return new WaitForSeconds(2.5f);
        HideRoundNumber();
        mainGameUIMoveScript.StartMoveUI(LerpType.Out, new Vector2(0, 250), Vector2.zero, 1.0f);
        TextHintFadeScript.FadeInUI(1.0f);

        int prevRoud = 0;
        for (int i = 0; i < 1000; i++) {
            cameraSystem.ResetCamera();
            cameraSystem.SetCameraTarget(null);

            foreach (var player in PlayerList) {
                currentTurnEnded = false;
                CurrentPlayerTurn = player;

                foreach (Player players in PlayerList) {
                    foreach (GameObject ant in players.AntList) {
                        ant.GetComponent<Ant>().FreezeMovement();
                    }
                }

                startTurnEvent.Invoke();
                yield return new WaitUntil(() => bbqScript.IsBurning == false && hose.IsSpraying == false);

                cameraSystem.SetCameraZoomingBool(true);
                turnTimerCoroutine = StartCoroutine(TurnTimer());
                playerTurnText.text = "Player " + player.playerInfo.playerNum.ToString();
                playerTurnText.color = player.playerInfo.playerColor;

                WeaponMenuText.spriteAsset = player.GetSpriteFromAction("WeaponMenu");
                FireWeaponText.spriteAsset = player.GetSpriteFromAction("FireWeapon");

                if (WeaponMenuText.spriteAsset != null) {
                    WeaponMenuText.text = "<sprite=0> - Weapons Menu";
                }
                else {
                    WeaponMenuText.text = player.GetKeybindForAction("WeaponMenu") + " - Weapons Menu";
                }

                if (FireWeaponText.spriteAsset != null) {
                    FireWeaponText.text = "<sprite=0> - Fire";
                }
                else {
                    FireWeaponText.text = player.GetKeybindForAction("FireWeapon") + " - Fire";
                }

                if (CurrentAntTurn.antInfo.IsQueen == true) {
                    QueenAttackText.spriteAsset = player.GetSpriteFromAction("QueenAttack");

                    if (QueenAttackText.spriteAsset != null) {
                        QueenAttackText.text = "<sprite=0> - Queen Attack";
                    }
                    else {
                        QueenAttackText.text = player.GetKeybindForAction("QueenAttack") + " - Queen Attack";
                    }

                    QueenAttackText.GetComponent<FadeScript>().FadeInUI(1.0f);
                }

                player.ResetFreeCamSetting();
                cameraSystem.ResetCamera();
                cameraSystem.SetCameraTarget(CurrentAntTurn.transform);

                enterObjectManager.StartTurnEvent();

                CurrentAntTurn.UnFreezeMovement();

                yield return new WaitUntil(() => currentTurnEnded == true);
            }
                        

            if (CurrentRound == maxRounds) {
				GameOver();
			} 
            else if(prevRoud != CurrentRound) {
                roundNumText.text = "Current Round: " + (CurrentRound + 1);

                cameraSystem.ResetCamera();
                cameraSystem.SetCameraTarget(null);

                mainGameUIMoveScript.StartMoveUI(LerpType.In, Vector2.zero, new Vector2(0, 250), 1.0f);
                TextHintFadeScript.FadeOutUI(1.0f);
                ShowRoundNumber();
                yield return new WaitForSeconds(2.5f);
                HideRoundNumber();
                mainGameUIMoveScript.StartMoveUI(LerpType.Out, new Vector2(0, 250), Vector2.zero, 1.0f);
                TextHintFadeScript.FadeInUI(1.0f);

                startRoundEvent.Invoke();

                prevRoud++;
            }
        }       
    }

    private IEnumerator TurnTimer() {
        PickAntTurn();

        while (currentTurnTime > 0) {
            if (turnTimerPaused == false) {
                currentTurnTime -= Time.deltaTime;
                SetTurnTimerText(currentTurnTime);
            }

            yield return null;
        }

        weaponManager.WaitTillWeaponsFinished();
        yield return new WaitUntil(() => weaponManager.WeaponsActive == false && cameraSystem.CameraDelayActive == false);
        EndTurn();
    }

    public void SetTurnTimerText(float time) {
        turnTimeText.text = "Time: " + TimeSpan.FromSeconds(time).ToString("ss");
    }

    public void PauseTurnTimer(float pauseDuration) {
        StartCoroutine(PauseTurnTimerCoroutine(pauseDuration));
    }

    private IEnumerator PauseTurnTimerCoroutine(float pauseDuration) {
        turnTimerPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        turnTimerPaused = false;
    }

    public void EndTurn() {
        StartCoroutine(EndTurnCoroutine());
    }

    private IEnumerator EndTurnCoroutine() {
        StopCoroutine(turnTimerCoroutine);

        yield return new WaitUntil(() => gameOver == false);

        if (CurrentAntTurn != null) {
            CurrentAntTurn.ApplyEffects();

            if (CurrentAntTurn.GetComponent<QueenBaseAntScript>() != null) {
                CheckIfQueenAttacked();
            }

            if (CurrentAntTurn.GetComponent<MummyScript>() != null) {
                CurrentAntTurn.GetComponent<MummyScript>().DecreaseTurnsAlive();
            }

            if (CurrentAntTurn.antInfo.IsQueen == true) {
                QueenAttackText.GetComponent<FadeScript>().FadeOutUI(0.5f);
            }
        }

        cameraSystem.IterateCameraTargets(1.0f);
        dropSystem.CheckDrop();
        yield return new WaitUntil(() => cameraSystem.CameraDelayActive == false);

        CheckIfAllAntsMoved();
        CurrentPlayerTurn = null;
        cameraSystem.SetCameraTarget(null);
        cameraSystem.SetCameraLookAtTarget(null);
        weaponManager.EndTurn();
        endTurnEvent.Invoke();
        onTurnEnded?.Invoke();
        yield return new WaitUntil(() => weaponManager.WeaponMenuOpen == false);

        if (allAntsMoved) {

            for (int i = 0; i < PlayerList.Count; i++) {
                PlayerList[i].ResetAnts();// sets all the ants back to not having moved
            }

            allAntsMoved = false; // ensures the round doesnt end until all ants have moved


            CurrentRound++;
            yield return new WaitUntil(() => dropSystem.IsDropping == false);
        }

        foreach (Player player in PlayerList) {
            foreach (GameObject ant in player.AntList) {
                ant.GetComponent<Ant>().FreezeMovement();
            }
        }

        currentTurnEnded = true;
        currentTurnTime = maxTurnTime;
    }

    public void GameOver() {
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine() {
        Debug.Log("Game Over!");
        gameOver = true;
        blackscreen.SetActive(true);
        blackscreen.GetComponent<CanvasGroup>().alpha = 0;
        blackscreen.GetComponent<FadeScript>().FadeInUI(2.0f);

        yield return new WaitUntil(() => blackscreen.GetComponent<CanvasGroup>().alpha == 1);
        SceneManager.LoadScene("GameOverScene");
    }

    //Decides which ant to use
    private void PickAntTurn() {
        CurrentAntTurn = CurrentPlayerTurn.GetAnt(CurrentAntTurn);

        if (CurrentAntTurn != null) {
            CurrentAntTurn.hasHadTurn = true;
        }
    }

    private void CheckIfAllAntsMoved() {
        int playersFinishedRound = 0;
        for (int i = 0; i < PlayerList.Count; i++) {
            CurrentAntTurn = PlayerList[i].GetAnt(CurrentAntTurn);
            if (CurrentAntTurn == null) {
                playersFinishedRound++;
            }
        }
        if (playersFinishedRound == PlayerList.Count) {
            allAntsMoved = true;
        }
    }

    public void ChangeQueenSpecialism(string queenType, Ant queenAnt) {
        switch (queenType) {
            case "Bullet":
                queenAnt.antInfo = queenAntSpecials.Where(x => x.queenType == AntSO.QueenType.Bullet).First();
                break;
            case "Bee":
                queenAnt.antInfo = queenAntSpecials.Where(x => x.queenType == AntSO.QueenType.Bee).First();
                break;
            case "Weaver":
                queenAnt.antInfo = queenAntSpecials.Where(x => x.queenType == AntSO.QueenType.Weaver).First();
                break;
            case "Ice":
                queenAnt.antInfo = queenAntSpecials.Where(x => x.queenType == AntSO.QueenType.Ice).First();
                break;
            case "Dracula":
                queenAnt.antInfo = queenAntSpecials.Where(x => x.queenType == AntSO.QueenType.Dracula).First();
                break;
            case "Pharaoh":
                queenAnt.antInfo = queenAntSpecials.Where(x => x.queenType == AntSO.QueenType.Pharaoh).First();
                break;
            default:
                Debug.LogError("Invalid Queen Ant - " + queenType);
                break;
        }
    }

    private void CheckIfQueenAttacked() {
		CurrentAntTurn.GetComponent<QueenBaseAntScript>().CheckAttackTurn();
	}

    public void HideRoundNumber() {
        roundNumText.GetComponent<FadeScript>().FadeOutUI(1.0f);
    }

    public void HideRoundNumber(float duration) {
        roundNumText.GetComponent<FadeScript>().FadeOutUI(duration);
    }

    public void ShowRoundNumber() {
        roundNumText.GetComponent<FadeScript>().FadeInUI(1.0f);
    }

    public void ShowRoundNumber(float duration) {
        roundNumText.GetComponent<FadeScript>().FadeInUI(duration);
    }
}