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
    [SerializeField] private GameObject skippingTurnText;
    [SerializeField] private PauseMenuScript pauseMenu;

    [field: Header("Text Hints")]
    [field: SerializeField] public FadeScript TextHintFadeScript { get; private set; }
    [field: SerializeField] public TMP_Text WeaponMenuText { get; private set; }
    [field: SerializeField] public TMP_Text FireWeaponText { get; private set; }
    [field: SerializeField] public TMP_Text QueenAttackText { get; private set; }

    private int numOfAnts = 2;
    private float currentTurnTime;
    private bool currentTurnEnded = false;
    private bool turnTimerPaused = false;
    private bool gameOver = false;

    public delegate void OnTurnEnded();
    public static OnTurnEnded onTurnEnded;

    public bool IsPaused { get; private set; } = false;
    public bool PauseInProgress { get; private set; } = false;
    public Player CurrentPlayerTurn { get; private set; } = null;
    public Ant CurrentAntTurn { get; private set; } = null; //Tracks which ant's turn it currently is
    public List<Player> PlayerList { get; private set; } = new List<Player>();
    public List<Ant> MovingAnts { get; private set; } = new List<Ant>();

    private Coroutine turnTimerCoroutine;
    private AudioSource audioSource;
    private AntSpawner antSpawner;
    private CameraSystem cameraSystem;
    private WeaponManager weaponManager;
    private WeaponDropSystem dropSystem;
    private BBQScript bbqScript;
    private Hose hose;
    private EnterObjectManager enterObjectManager;
    private Dictionary<int, List<Ant>> antDictionary = new Dictionary<int, List<Ant>>();

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
        audioSource = GetComponent<AudioSource>();

        currentTurnTime = maxTurnTime;
        StartCoroutine(StartLevelCoroutine());
    }

    //Called at the start of the level - camera pan & level name text
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

        StartCoroutine(RoundsCoroutine());
    }

    //Spawn in all ants
    private void SpawnAllAnts() {
        for (int i = 0; i < PlayerList.Count; i++) {
            for (int j = 0; j < numOfAnts; j++) {
                GameObject newAnt = antSpawner.SpawnAnt(baseAntPrefab);
                PlayerList[i].AddNewAnt(newAnt);
                newAnt.GetComponent<BaseAntScript>().ChangeAntColors(PlayerList[i].playerInfo.playerColor);
                newAnt.GetComponent<Ant>().SetHealthTextColor(PlayerList[i].playerInfo.playerColor);

                switch (i) {
                    case 0:
                        newAnt.GetComponent<Ant>().ownedPlayer = Ant.PlayerList.Player1;
                        newAnt.GetComponent<Ant>().SetAntOwner(i + 1);
                        break;
                    case 1:
                        newAnt.GetComponent<Ant>().ownedPlayer = Ant.PlayerList.Player2;
                        newAnt.GetComponent<Ant>().SetAntOwner(i + 1);
                        break;
                    case 2:
                        newAnt.GetComponent<Ant>().ownedPlayer = Ant.PlayerList.Player3;
                        newAnt.GetComponent<Ant>().SetAntOwner(i + 1);
                        break;
                    case 3:
                        newAnt.GetComponent<Ant>().ownedPlayer = Ant.PlayerList.Player4;
                        newAnt.GetComponent<Ant>().SetAntOwner(i + 1);
                        break;
                }
            }

            SpawnQueen(i); //Spawn queen ant
        }

        enterObjectManager.OnAllAntsSpawned();
    }

    //Spawn queen ant
    private void SpawnQueen(int playerNum) {
        GameObject newQueen = antSpawner.SpawnAnt(queenPrefab);

        ChangeQueenSpecialism(PlayerList[playerNum].playerInfo.queenType, newQueen.GetComponent<Ant>());
        newQueen.GetComponent<QueenBaseAntScript>().InitialiseQueenAttack();
        newQueen.GetComponent<QueenBaseAntScript>().SetAntColor(PlayerList[playerNum].playerInfo.playerColor);
        newQueen.GetComponent<Ant>().SetHealthTextColor(PlayerList[playerNum].playerInfo.playerColor);
        newQueen.GetComponent<QueenBaseAntScript>().ChangeQueenMesh(PlayerList[playerNum].playerInfo.queenType);
        PlayerList[playerNum].AddQueen(newQueen);

        switch (playerNum) {
            case 0:
                newQueen.GetComponent<Ant>().ownedPlayer = Ant.PlayerList.Player1;
                newQueen.GetComponent<Ant>().SetAntOwner(playerNum + 1);
                break;
            case 1:
                newQueen.GetComponent<Ant>().ownedPlayer = Ant.PlayerList.Player2;
                newQueen.GetComponent<Ant>().SetAntOwner(playerNum + 1);
                break;
            case 2:
                newQueen.GetComponent<Ant>().ownedPlayer = Ant.PlayerList.Player3;
                newQueen.GetComponent<Ant>().SetAntOwner(playerNum + 1);
                break;
            case 3:
                newQueen.GetComponent<Ant>().ownedPlayer = Ant.PlayerList.Player4;
                newQueen.GetComponent<Ant>().SetAntOwner(playerNum + 1);
                break;
        }
    }

    private IEnumerator LevelTextCoroutine() {
        yield return new WaitForSeconds(2.0f);
        levelNameText.GetComponent<FadeScript>().FadeOutUI(2.0f);
    }

    //Used for handling all turn logic
    private IEnumerator RoundsCoroutine() {
        for (int roundCounter = 0; roundCounter < maxRounds; roundCounter++) {
            roundNumText.text = "Current Round: " + (roundCounter + 1);
            cameraSystem.ResetCamera();
            cameraSystem.SetCameraTarget(null);

            ShowRoundNumber();
            yield return new WaitForSeconds(2.5f);
            HideRoundNumber();
            ShowMainUI();

            startRoundEvent.Invoke(); //Start of round events

            //Start of round get all ants by player
            foreach (Player player in PlayerList) {
                List<Ant> playerAnts = new List<Ant>();

                foreach (GameObject ant in player.AntList) {
                    playerAnts.Add(ant.GetComponent<Ant>());
                }

                if (player.QueenAnt != null) {
                    playerAnts.Add(player.QueenAnt.GetComponent<Ant>());
                }

                if (playerAnts.Count > 0) {
                    antDictionary.Add(player.playerInfo.playerNum, playerAnts);
                }
            }

            //Keep looping through ants till all ants have had a turn
            while (antDictionary.Count > 0) {
                foreach (Player player in PlayerList) {
                    antDictionary.TryGetValue(player.playerInfo.playerNum, out List<Ant> antList);

                    if (antList == null || antList.Count == 0) {
                        continue;
                    }

                    CurrentPlayerTurn = player;
                    currentTurnEnded = false;
                    CurrentAntTurn = antList[0];                    

                    foreach (Player players in PlayerList) {
                        if (players == player) {
                            players.GetEventSystem().enabled = true;
                        }
                        else {
                            players.GetEventSystem().enabled = false;
                        }
                    }

                    startTurnEvent.Invoke(); //Start of turn events
                    yield return new WaitUntil(() => bbqScript.IsBurning == false && hose.IsSpraying == false);

                    StartTurnFunctionality(player);

                    yield return new WaitUntil(() => currentTurnEnded == true);
                }
            }
        }

        GameOver(); //Game ends if all rounds are complete
    }

    //Set up functionality called at start of turn
    private void StartTurnFunctionality(Player player) {
        cameraSystem.SetCameraZoomingBool(true);
        turnTimerCoroutine = StartCoroutine(TurnTimer());
        playerTurnText.text = "Player " + player.playerInfo.playerNum.ToString();
        playerTurnText.color = player.playerInfo.playerColor;

        ShowMainUI();
        DisplayButtonHints(player);

        player.ResetFreeCamSetting();
        cameraSystem.ResetCamera();
        cameraSystem.SetCameraTarget(CurrentAntTurn.transform);
        CurrentAntTurn.SetCanMove(true);
        CurrentPlayerTurn.hasSkippedTurn = false;

        enterObjectManager.StartTurnEvent();
    }

    //Display correct hints to the player
    private void DisplayButtonHints(Player player) {
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

        FireWeaponText.GetComponent<FadeScript>().FadeOutUI(0.25f);
        WeaponMenuText.GetComponent<FadeScript>().FadeInUI(0.25f);

        if (CurrentAntTurn.antInfo.IsQueen == true) {
            QueenAttackText.spriteAsset = player.GetSpriteFromAction("QueenAttack");

            if (QueenAttackText.spriteAsset != null) {
                QueenAttackText.text = "<sprite=0> - Queen Attack";
            }
            else {
                QueenAttackText.text = player.GetKeybindForAction("QueenAttack") + " - Queen Attack";
            }

            ShowMainUI();
            QueenAttackText.GetComponent<FadeScript>().FadeInUI(1.0f);
        }
        else {
            QueenAttackText.GetComponent<FadeScript>().FadeOutUI(0.5f);
        }
    }

    //Timer for turns
    private IEnumerator TurnTimer() {
        bool countdownStarted = false;

        while (currentTurnTime > 0) {
            if (turnTimerPaused == false) {
                currentTurnTime -= Time.deltaTime;
                SetTurnTimerText(currentTurnTime);
            }

            if (currentTurnTime <= 11 && countdownStarted == false) {
                countdownStarted = true;
                audioSource.Play();
            }

            yield return null;
        }

        StartCoroutine(weaponManager.WaitTillWeaponsFinishedCoroutine());
        yield return new WaitUntil(() => weaponManager.WeaponsActive == false && cameraSystem.CameraDelayActive == false);
        StartCoroutine(EndTurnCoroutine());
    }

    public void SetTurnTimerText(float time) {
        turnTimeText.text = "Time: " + TimeSpan.FromSeconds(time).ToString("ss");
    }

    public void SetPauseTurnTimer(bool _turnTimerPaused) {
        turnTimerPaused = _turnTimerPaused;
    }

    public IEnumerator PauseTurnTimerCoroutine(float pauseDuration) {
        turnTimerPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        turnTimerPaused = false;
    }

    //Called when a turn ends
    public IEnumerator EndTurnCoroutine() {
        StopCoroutine(turnTimerCoroutine);

        if (audioSource.isPlaying) { 
            audioSource.Stop();
        }

        HideMainUI(0.25f);
        CurrentAntTurn.SetCanMove(false);
        yield return new WaitForSeconds(0.25f);

        yield return new WaitUntil(() => gameOver == false);
        yield return new WaitUntil(() => cameraSystem.CameraDelayActive == false);
        yield return new WaitUntil(() => MovingAnts.Count == 0);

        if (CurrentAntTurn != null) {
            if (CurrentAntTurn.effects.Count > 0) {
                CurrentAntTurn.ApplyEffects();
                yield return new WaitForSeconds(1.0f);
            }

            if (CurrentAntTurn.GetComponent<QueenBaseAntScript>() != null) {
                CurrentAntTurn.GetComponent<QueenBaseAntScript>().CheckAttackTurn();
            }

            if (CurrentAntTurn.GetComponent<MummyScript>() != null) {
                CurrentAntTurn.GetComponent<MummyScript>().DecreaseTurnsAlive();
            }

            if (CurrentAntTurn.antInfo.IsQueen == true) {
                QueenAttackText.GetComponent<FadeScript>().FadeOutUI(0.5f);
            }
        }
        
        cameraSystem.SetCameraLookAtTarget(null);
        cameraSystem.IterateCameraTargets(1.0f);
        yield return new WaitUntil(() => cameraSystem.CameraDelayActive == false);

        yield return null;
        yield return new WaitUntil(() => gameOver == false);

        dropSystem.CheckDrop();
        yield return new WaitUntil(() => cameraSystem.CameraDelayActive == false);

        if (CurrentAntTurn != null) {
            CurrentAntTurn.ResetAnimation();
            RemoveAntFromDictionary(CurrentPlayerTurn.playerInfo.playerNum, CurrentAntTurn); //Remove ant from list of available ants
        }

        CurrentPlayerTurn = null;
        cameraSystem.SetCameraTarget(null);
        weaponManager.EndTurn();
        onTurnEnded?.Invoke();
        yield return new WaitUntil(() => weaponManager.WeaponMenuOpen == false);

        endTurnEvent.Invoke();
        yield return new WaitUntil(() => cameraSystem.CameraDelayActive == false);
        yield return new WaitUntil(() => dropSystem.IsDropping == false);

        currentTurnEnded = true;
        currentTurnTime = maxTurnTime;
    }

    public void RemoveAntFromDictionary(int playerNum, Ant ant) {
        antDictionary.TryGetValue(playerNum, out List<Ant> antList);
        List<Ant> newAntList = antList;

        if (newAntList.Contains(ant)) {
            newAntList.Remove(ant);
        }

        antDictionary.Remove(playerNum);

        if (newAntList.Count > 0) {
            antDictionary.Add(playerNum, newAntList);
        }
    }

    public void GameOver() {
        StartCoroutine(GameOverCoroutine());
    }

    //Called when the game ends
    private IEnumerator GameOverCoroutine() {
        gameOver = true;
        blackscreen.SetActive(true);
        blackscreen.GetComponent<CanvasGroup>().alpha = 0;
        blackscreen.GetComponent<FadeScript>().FadeInUI(1.0f);

		yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("GameOverScene");
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

    public void ShowSkippingTurnText() {
        skippingTurnText.SetActive(true);
    }

    public void HideSkippingTurnText() {
        skippingTurnText.SetActive(false);
    }

    public void HideMainUI() {
        if (mainGameUIMoveScript.GetComponent<RectTransform>().anchoredPosition.y < 5) {
            mainGameUIMoveScript.StartMoveUI(LerpType.In, Vector2.zero, new Vector2(0, 250), 1.0f);
            TextHintFadeScript.FadeOutUI(1.0f);
        }
    }

    public void HideMainUI(float time) {
        if (mainGameUIMoveScript.GetComponent<RectTransform>().anchoredPosition.y < 5) {
            mainGameUIMoveScript.StartMoveUI(LerpType.In, Vector2.zero, new Vector2(0, 250), time);
            TextHintFadeScript.FadeOutUI(time);
        }
    }

    public void ShowMainUI() {
        mainGameUIMoveScript.StartMoveUI(LerpType.Out, new Vector2(0, 250), Vector2.zero, 1.0f);
        TextHintFadeScript.FadeInUI(1.0f);
    }

    //Called when the game is paused
    public void PauseGame() {
        PauseInProgress = true;
        IsPaused = true;

        CurrentAntTurn.SetCanMove(false);
        StartCoroutine(weaponManager.ForceCloseWeaponMenuCoroutine());
        HideMainUI();
        SetPauseTurnTimer(true);
        pauseMenu.gameObject.SetActive(true);
        pauseMenu.DisplayPauseMenu();
        StartCoroutine(PauseInProgressCoroutine(false));
    }

    private IEnumerator PauseInProgressCoroutine(bool unPausing) {
        yield return new WaitForSeconds(1.0f);
        PauseInProgress = false;

        if (unPausing) {
            IsPaused = false;
            pauseMenu.gameObject.SetActive(false);
            SetPauseTurnTimer(false);
            CurrentAntTurn.SetCanMove(true);
        }
    }

    public void UnPauseGame() {
        PauseInProgress = true;

        pauseMenu.HidePauseMenu();
        ShowMainUI();
        StartCoroutine(PauseInProgressCoroutine(true));
    }

    public void AddMovingAnt(Ant ant) {
        StartCoroutine(AddMovingAntCoroutine(ant));
    }

    private IEnumerator AddMovingAntCoroutine(Ant ant) {
        yield return new WaitForSeconds(0.25f);
        MovingAnts.Add(ant);
    }

    public void RemoveMovingAnt(Ant ant) { 
        MovingAnts.Remove(ant);
    }
}