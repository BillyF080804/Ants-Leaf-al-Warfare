using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private string levelName;
    [SerializeField] private int numOfRounds = 3;
    [SerializeField] private int maxTurnTime = 20;

    [Header("Ant Spawning Settings")]
    [SerializeField] private float mapMinX = -10.0f;
    [SerializeField] private float mapMaxX = 10.0f;
    [SerializeField] private float minDistanceBetweenAnts = 3.0f;

    [field: Header("Gamemode Settings")]
    public int DamageToDealOnQueenDeath { get; private set; } = 10;

    [Header("Event Settings")]
    [SerializeField] private UnityEvent startTurnEvent;
    [SerializeField] private UnityEvent endTurnEvent;

    [Header("Ant Prefabs")]
    [SerializeField] private GameObject antPrefab;
    [SerializeField] private GameObject queenPrefab;

    [Header("UI")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private TMP_Text turnTimeText;
    [SerializeField] private GameObject blackscreen;
    [SerializeField] private GameObject levelNameText;
    [SerializeField] private GameObject queenHealthUIPrefab;
    [SerializeField] private List<Sprite> queenAntHealthUIVariants;

    private int numOfAnts = 2;
    private float currentTurnTime;
    private bool currentTurnEnded = false;
    private bool turnTimerPaused = false;
    private bool allAntsMoved = false;
    private bool gameOver = false;

    public int CurrentRound { get; private set; } = 0;
    public string Gamemode { get; private set; } = string.Empty;
    public Player CurrentPlayerTurn { get; private set; } = null;
    public Ant CurrentAntTurn { get; private set; } = null; //Tracks which ant's turn it currently is
    public List<GameObject> QueenHealthUI { get; private set; } = new List<GameObject>();
    public List<Player> PlayerList { get; private set; } = new List<Player>();

    private Coroutine turnTimerCoroutine;
    private CameraSystem cameraSystem;
    private WeaponManager weaponManager;
    private WeaponDropSystem dropSystem;


    [SerializeField] TextMeshProUGUI tempText;

    private void Start() {
        numOfAnts = LoadingData.numOfAnts;
        PlayerList = LoadingData.playerList;
        Gamemode = LoadingData.gamemode;
        dropSystem = FindFirstObjectByType<WeaponDropSystem>();
        weaponManager = FindFirstObjectByType<WeaponManager>();
        cameraSystem = FindFirstObjectByType<CameraSystem>();

        currentTurnTime = maxTurnTime;
        StartCoroutine(StartLevelCoroutine());
    }

    private IEnumerator StartLevelCoroutine() {
        StartCoroutine(LevelTextCoroutine());
        yield return new WaitUntil(() => levelNameText.activeSelf == false);

        StartCoroutine(SpawnQueenCoroutine());
        yield return new WaitUntil(() => PlayerList.All(x => x.ConfirmedQueenSpawn == true));

        SpawnAnts();
        SpawnQueenAntHealthUI();
        yield return new WaitUntil(() => QueenHealthUI.Count == PlayerList.Count);

        StartCoroutine(StartGame());
    }

    private void SpawnAnts() {
        for (int i = 0; i < PlayerList.Count; i++) {
            for (int j = 0; j < numOfAnts; j++) {
                GameObject newAnt = Instantiate(antPrefab, GetAntSpawnPoint(), Quaternion.identity);
                PlayerList[i].AddNewAnt(newAnt);
                newAnt.GetComponent<Ant>().ownedPlayer = (Ant.PlayerList)i;
                newAnt.GetComponentInChildren<MeshRenderer>().material.color = PlayerList[i].playerInfo.playerColor;
            }
        }
    }

    private IEnumerator SpawnQueenCoroutine() {
        for (int i = 0; i < PlayerList.Count; i++) {
            GameObject newQueen = Instantiate(queenPrefab, GetAntSpawnPoint(), Quaternion.identity);
            newQueen.GetComponentInChildren<MeshRenderer>().material.color = PlayerList[i].playerInfo.playerColor;
            PlayerList[i].AddQueen(newQueen);
            PlayerList[i].AllowPlayerToSpawnQueen();
            cameraSystem.SetCameraTarget(newQueen.transform);

            yield return new WaitUntil(() => PlayerList[i].ConfirmedQueenSpawn == true);
        }
    }

    private void SpawnQueenAntHealthUI() {
        for (int i = 0; i < PlayerList.Count; i++) {
            GameObject newHealthUI = Instantiate(queenHealthUIPrefab, mainCanvas.transform);
            QueenHealthUI.Add(newHealthUI);
            Image mainUI = newHealthUI.transform.GetChild(2).GetComponent<Image>();
            RectTransform textTransform = newHealthUI.transform.GetChild(3).GetComponent<RectTransform>();
            RectTransform backgroundTransform = newHealthUI.transform.GetChild(0).GetComponent<RectTransform>();
            RectTransform queenImageTransform = newHealthUI.transform.GetChild(1).GetComponent<RectTransform>();

            switch (i) {
                case 0:
                    newHealthUI.GetComponent<RectTransform>().localPosition = new Vector2(-785, 445);
                    backgroundTransform.localPosition = new Vector2(-75.5f, 0);
                    queenImageTransform.localPosition = new Vector2(-75.5f, 0);                    
                    textTransform.localPosition = new Vector2(70, -45);
                    textTransform.GetComponent<TMP_Text>().text = "Queen Health: 100";
                    mainUI.sprite = queenAntHealthUIVariants[i];
                    mainUI.color = PlayerList[i].playerInfo.playerColor;
                    break;
                case 1:
                    newHealthUI.GetComponent<RectTransform>().localPosition = new Vector2(785, 445);
                    backgroundTransform.localPosition = new Vector2(75.5f, 0);
                    queenImageTransform.localPosition = new Vector2(75.5f, 0);
                    queenImageTransform.Rotate(new Vector3(0, 180, 0));
                    textTransform.localPosition = new Vector2(-70, -45);
                    textTransform.GetComponent<TMP_Text>().text = "Queen Health: 100";
                    mainUI.sprite = queenAntHealthUIVariants[i];
                    mainUI.color = PlayerList[i].playerInfo.playerColor;
                    break;
                case 2:
                    newHealthUI.GetComponent<RectTransform>().localPosition = new Vector2(-785, -445);
                    backgroundTransform.localPosition = new Vector2(-75.5f, 0);
                    queenImageTransform.localPosition = new Vector2(-75.5f, 0);
                    textTransform.localPosition = new Vector2(70, -45);
                    textTransform.GetComponent<TMP_Text>().text = "Queen Health: 100";
                    mainUI.sprite = queenAntHealthUIVariants[i];
                    mainUI.color = PlayerList[i].playerInfo.playerColor;
                    break;
                case 3:
                    newHealthUI.GetComponent<RectTransform>().localPosition = new Vector2(785, -445);
                    backgroundTransform.localPosition = new Vector2(75.5f, 0);
                    queenImageTransform.localPosition = new Vector2(75.5f, 0);
                    queenImageTransform.Rotate(new Vector3(0, 180, 0));
                    textTransform.localPosition = new Vector2(-70, -45);
                    textTransform.GetComponent<TMP_Text>().text = "Queen Health: 100";
                    mainUI.sprite = queenAntHealthUIVariants[i];
                    mainUI.color = PlayerList[i].playerInfo.playerColor;
                    break;
            }
        }
    }

    private Vector3 GetAntSpawnPoint() {
        bool validSpawn = false;
        Vector3 spawnPos = Vector3.zero;
        int spawnAttempts = 0;

        while (validSpawn == false) {
            spawnAttempts++;
            spawnPos = new Vector3(Random.Range(mapMinX, mapMaxX), 30.0f, 0);

            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit ray, 35.0f)) {
                spawnPos = new Vector3(ray.point.x, ray.point.y + 1.0f, ray.point.z);
            }

            Collider[] colliders = Physics.OverlapSphere(spawnPos, minDistanceBetweenAnts).Where(x => x.CompareTag("Player")).ToArray();

            if (colliders.Count() == 0) {
                validSpawn = true;
            }
            else if (spawnAttempts == 10) {
                Debug.LogError("ERROR: 10 Attempts to spawn ant. Ant Spawning Failed. Spawning at most recent attempt.\nThis can be avoided by either decreasing the distance between spawns or increasing map size.");
                validSpawn = true;
            }
        }

        return spawnPos;
    }

    private IEnumerator LevelTextCoroutine() {
        levelNameText.GetComponent<TMP_Text>().text = levelName;

        yield return new WaitForSeconds(1.0f);
        levelNameText.GetComponent<FadeScript>().FadeOutUI(2.0f, levelNameText);
    }

    private IEnumerator StartGame() {
        // PickAntTurn();

        for (int i = 0; i < numOfRounds; i++) {
            tempText.text = "Current Round: " + (CurrentRound + 1);

            foreach (var player in PlayerList) {
                currentTurnEnded = false;
                CurrentPlayerTurn = player;
                turnTimerCoroutine = StartCoroutine(TurnTimer());
                cameraSystem.SetCameraTarget(CurrentAntTurn.transform);
                startTurnEvent.Invoke();

                yield return new WaitUntil(() => currentTurnEnded == true);
            }

            if (i == CurrentRound && CurrentRound != numOfRounds) {
                i--;
            }
        }
    }

    private IEnumerator TurnTimer() {
        PickAntTurn();

        while (currentTurnTime > 0) {
            if (turnTimerPaused == false) {
                currentTurnTime -= Time.deltaTime;
                turnTimeText.text = "Time: " + TimeSpan.FromSeconds(currentTurnTime).ToString("ss");
            }

            yield return null;
        }

        weaponManager.WaitTillWeaponsFinished();
        yield return new WaitUntil(() => weaponManager.WeaponsActive == false && cameraSystem.CameraDelayActive == false);
        EndTurn();
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
        CurrentPlayerTurn.StopSkipTurn();

        yield return new WaitUntil(() => gameOver == false);

        CurrentAntTurn.ApplyEffects();

        if (CurrentAntTurn.GetComponent<QueenAntScript>() != null) {
            CheckIfQueenAttacked();

		}

        CheckIfAllAntsMoved();
        CurrentPlayerTurn = null;
        cameraSystem.SetCameraTarget(null);
        weaponManager.EndTurn();
        endTurnEvent.Invoke();
        yield return new WaitUntil(() => weaponManager.WeaponMenuOpen == false);


        if (allAntsMoved) { //CurrentPlayerTurn == playerList[playerList.Count - 1] && 

            for (int i = 0; i < PlayerList.Count; i++) {
                PlayerList[i].ResetAnts();// sets all the ants back to not having moved
            }

            allAntsMoved = false; // ensures the round doesnt end until all ants have moved


            CurrentRound++;
            dropSystem.CheckDrop();
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

    private void CheckIfQueenAttacked() {
		CurrentAntTurn.GetComponent<QueenAntScript>().CheckAttackTurn();
	}
}