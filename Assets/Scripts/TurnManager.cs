using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private string levelName;
    [SerializeField] private int numOfRounds;
    [SerializeField] private int maxTurnTime;

    [Header("UI")]
    [SerializeField] private GameObject levelNameText;

    private float currentTurnTime;
    private bool currentTurnEnded = false;
    public Player CurrentPlayerTurn { get; private set; } = null;
    private List<Player> playerList = new List<Player>();

    private void Start() {
        playerList = LoadingData.playerList;
        StartCoroutine(LevelTextCoroutine());
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
        while (currentTurnTime < maxTurnTime) {
            currentTurnTime += Time.deltaTime;
            yield return null;
        }

        EndTurn();
    }

    public void EndTurn() {
        StopCoroutine(nameof(TurnTimer));
        currentTurnEnded = true;
        currentTurnTime = 0.0f;
        CurrentPlayerTurn = null;
    }
}