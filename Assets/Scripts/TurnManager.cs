using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    private int numOfAnts = 2;
    private float currentTurnTime;
    private bool currentTurnEnded = false;

    public int CurrentRound { get; private set; } = 1;
    public Player CurrentPlayerTurn { get; private set; } = null;

    //Tracks which ant's turn it currently is
    public AntScript CurrentAntTurn { get; private set; } = null;
    public bool allAntsMoved = false;

	private WeaponDropSystem dropSystem;
    private List<Player> playerList = new List<Player>();

    private void Start() {
        numOfAnts = LoadingData.numOfAnts;
        playerList = LoadingData.playerList;
        dropSystem = FindFirstObjectByType<WeaponDropSystem>();
        SpawnAnts();
        SpawnQueen();

		StartCoroutine(LevelTextCoroutine());
    }

    private void SpawnAnts() {
        for (int i = 0; i < playerList.Count; i++) {
            for (int j = 0; j < numOfAnts; j++) { 
                GameObject newAnt = Instantiate(antPrefab, GetAntSpawnPoint(), Quaternion.identity);
                playerList[i].AddNewAnt(newAnt);
                newAnt.GetComponent<AntScript>().ownedPlayer = (AntScript.PlayerList)i;
                newAnt.GetComponent<MeshRenderer>().material.color = playerList[i].playerInfo.playerColor;
            }
        }
    }

    private void SpawnQueen() {
        for (int i = 0; i < playerList.Count; i++) {
            GameObject newQueen = Instantiate(queenPrefab, GetAntSpawnPoint() , Quaternion.identity);
            playerList[i].AddQueen(newQueen);
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
        for (int i = 0; i < numOfRounds; i++) {
            foreach (var player in playerList) {
                currentTurnEnded = false;
                CurrentPlayerTurn = player;
                StartCoroutine(TurnTimer());

                yield return new WaitUntil(() => currentTurnEnded == true);
            }
        }
    }

	private IEnumerator TurnTimer() {
		PickAntTurn();

		while (currentTurnTime < maxTurnTime) {
            currentTurnTime += Time.deltaTime;
            yield return null;
        }

        EndTurn();
    }

    public void EndTurn() {
        StopCoroutine(nameof(TurnTimer));

        if (CurrentPlayerTurn == playerList[playerList.Count - 1] && allAntsMoved) {
            allAntsMoved = false; // ensures the round doesnt end until all ants have moved

            for (int i = 0;i < playerList.Count;i++) {
                playerList[i].ResetAnts();// sets all the ants back to not having moved
			}

            CurrentRound++;
            dropSystem.CheckDrop();
        }

        currentTurnEnded = true;
        currentTurnTime = 0.0f;
        CurrentPlayerTurn = null;
	}

    //Decides which ant to use
	void PickAntTurn() {
        CurrentAntTurn = CurrentPlayerTurn.GetAnt(CurrentAntTurn);

        if (CurrentAntTurn != null) {
            CurrentAntTurn.hasHadTurn = true;
        } 
        else {
            allAntsMoved = true;
        }
	}
}