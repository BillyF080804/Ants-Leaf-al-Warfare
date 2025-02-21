using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private string levelName;
    [SerializeField] private int numOfRounds;
    [SerializeField] private int maxTurnTime;

    [Header("Ant Prefabs")]
    [SerializeField] private GameObject antPrefab;
    [SerializeField] private GameObject queenPrefab;

    [Header("UI")]
    [SerializeField] private GameObject levelNameText;
    [SerializeField] private TMP_Text turnTimeText;
    [SerializeField] private GameObject queenHealthUIPrefab;
    [SerializeField] private Canvas mainCanvas;
    [field: SerializeField] public List<GameObject> QueenHealthUI { get; private set; }

    private int numOfAnts = 2;
    private float currentTurnTime;
    private bool currentTurnEnded = false;
    private bool turnTimerPaused = false;

    public int CurrentRound { get; private set; } = 0;
    public Player CurrentPlayerTurn { get; private set; } = null;

    //Tracks which ant's turn it currently is
    public Ant CurrentAntTurn { get; private set; } = null;
    public bool allAntsMoved = false;

    private Coroutine turnTimerCoroutine;
    private CameraSystem cameraSystem;
    private WeaponManager weaponManager;
    private WeaponDropSystem dropSystem;
    public List<Player> PlayerList { get; private set; } = new List<Player>();


    [SerializeField] TextMeshProUGUI tempText;

    private void Start() {
        numOfAnts = LoadingData.numOfAnts;
        PlayerList = LoadingData.playerList;
        dropSystem = FindFirstObjectByType<WeaponDropSystem>();
        weaponManager = FindFirstObjectByType<WeaponManager>();
        cameraSystem = FindFirstObjectByType<CameraSystem>();

        currentTurnTime = maxTurnTime;

        SpawnQueenAntHealthUI();
        SpawnAnts();
        SpawnQueen();

        StartCoroutine(LevelTextCoroutine());
    }

    private void SpawnAnts() {
        for (int i = 0; i < PlayerList.Count; i++) {
            for (int j = 0; j < numOfAnts; j++) {
                GameObject newAnt = Instantiate(antPrefab, GetAntSpawnPoint(), Quaternion.identity);
                PlayerList[i].AddNewAnt(newAnt);
                newAnt.GetComponent<Ant>().ownedPlayer = (Ant.PlayerList)i;
                newAnt.GetComponent<MeshRenderer>().material.color = PlayerList[i].playerInfo.playerColor;
            }
        }
    }

    private void SpawnQueen() {
        for (int i = 0; i < PlayerList.Count; i++) {
            GameObject newQueen = Instantiate(queenPrefab, GetAntSpawnPoint(), Quaternion.identity);
            PlayerList[i].AddQueen(newQueen);
        }
    }

    private void SpawnQueenAntHealthUI() {
        for (int i = 0; i < PlayerList.Count; i++) {
            GameObject newHealthUI = Instantiate(queenHealthUIPrefab, mainCanvas.transform);
            QueenHealthUI.Add(newHealthUI);

            switch (i) {
                case 0:
                    newHealthUI.GetComponent<RectTransform>().localPosition = new Vector2(-785, 415);
                    newHealthUI.GetComponentInChildren<TMP_Text>().text = "Player 1 Queen Ant Health: 100";
                    break;
                case 1:
                    newHealthUI.GetComponent<RectTransform>().localPosition = new Vector2(785, 415);
                    newHealthUI.GetComponentInChildren<TMP_Text>().text = "Player 2 Queen Ant Health: 100";
                    break;
                case 2:
                    newHealthUI.GetComponent<RectTransform>().localPosition = new Vector2(-785, -415);
                    newHealthUI.GetComponentInChildren<TMP_Text>().text = "Player 3 Queen Ant Health: 100";
                    break;
                case 3:
                    newHealthUI.GetComponent<RectTransform>().localPosition = new Vector2(785, -415);
                    newHealthUI.GetComponentInChildren<TMP_Text>().text = "Player 4 Queen Ant Health: 100";
                    break;
            }
        }
    }

    private Vector3 GetAntSpawnPoint() {
        return new Vector3(Random.Range(-10f, 10f), 1, 0);
    }

    private IEnumerator LevelTextCoroutine() {
        levelNameText.GetComponent<TMP_Text>().text = levelName;

        yield return new WaitForSeconds(1.0f);
        levelNameText.GetComponent<FadeScript>().FadeOutUI(2.0f, levelNameText);
        yield return new WaitUntil(() => levelNameText.activeSelf == false);

        StartCoroutine(StartGame());
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
		Player currentPlayerTemp = CurrentPlayerTurn.GetComponent<Player>();
        CheckIfAllAntsMoved();
        CurrentPlayerTurn = null;
        cameraSystem.SetCameraTarget(null);
        weaponManager.EndTurn();
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
}