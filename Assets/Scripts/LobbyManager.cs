using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private PlayerInputManager inputManager;

    [Header("Player Cards")]
    [SerializeField] private Transform playerCardHolder;
    [SerializeField] private PlayerCardInfo playerCard;

    [Header("Other UI")]
    [SerializeField] private TMP_Text playersJoinedText;
    [SerializeField] private TMP_Dropdown gamemodeDropdown;
    [SerializeField] private LoadingUI loadingUIPrefab;

    [Header("Scene Info")]
    [SerializeField] private List<SceneAsset> availableScenes;
    [SerializeField] private TMP_Dropdown sceneDropdown;

    private string gamemode;
    private string sceneToLoad;
    private int expectedPlayerCount = 0;
    private int numOfAnts = 2;
    private bool loadingUISpawned = false;

    private Coroutine readyCountdownCoroutine = null;
    private List<PlayerCardInfo> playerCardList = new List<PlayerCardInfo>();
    private List<Player> playerList = new List<Player>();

    private void Start() {
        List<string> sceneNames = new List<string>();
        foreach (SceneAsset scene in availableScenes) {
            sceneNames.Add(scene.name);
        }
        sceneDropdown.AddOptions(sceneNames);

        UpdatePlayerCount(0);
        UpdateSceneToLoad(0);
        UpdateGameMode(0);
    }

    //Called when a player changes the number of players in the game
    public void UpdatePlayerCount(int value) {
		switch (value) {
			case 0: {
				expectedPlayerCount = 1;
				break;
			}
			case 1: {
				expectedPlayerCount = 2;
				break;
			}
			case 2: {
				expectedPlayerCount = 3;
				break;
			}
			case 3: {
				expectedPlayerCount = 4;
				break;
			}
		}

		foreach (var player in playerCardList) { 
            Destroy(player.card);
        }

        foreach (var player in playerList) {
            Destroy(player.gameObject);
        }

        playerCardList.Clear();
        playerList.Clear();
        playersJoinedText.text = "Waiting On Players To Join . . .";

        for (int i = 0; i < expectedPlayerCount; i++) {
            PlayerCardInfo newCard = Instantiate(playerCard, playerCardHolder);
            newCard.name = "PlayerCard" + (i + 1);

            playerCardList.Add(newCard.GetComponent<PlayerCardInfo>());
            playerCardList[i].playerNum = i + 1;

            if (i != 0) {
                ChangeColor(playerCardList[i].playerNum);  
            }
            else {
                playerCardList[i].colorBand.color = ChooseNewColor();
            }
        }
    }

    //Called when a player presses the join button on their controller
    public void OnPlayerJoined(PlayerInput input) {
        if (playerList.Count < expectedPlayerCount) {
            playerList.Add(input.gameObject.GetComponent<Player>());

            int playerNum = playerList.Count;
            input.gameObject.name = "Player" + playerNum;

            Player player = playerList[playerNum - 1];
            PlayerCardInfo playerCard = playerCardList[playerNum - 1];

            player.playerInfo.playerNum = playerNum;
            player.playerInfo.playerInput = input;
            player.playerInfo.playerColor = playerCardList.Where(x => x.playerNum == playerNum).First().colorBand.color;
            playerCard.textHint.text = "Press " + input.actions.FindAction("ReadyUp").GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions) + " To Ready Up";
            playerCard.colorChangePrompt.text = input.actions.FindAction("ChangeColor").GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
            playerCard.colorChangePrompt.gameObject.SetActive(true);

            if (playerList.Count == expectedPlayerCount) {
                playersJoinedText.text = "Waiting On Players To Ready Up . . .";
            }
        }
    }

    public void UpdateAntCount(int _numOfAnts) {
        switch (_numOfAnts) {
            case 0: {
                numOfAnts = 2;
                break;
            }
            case 1: {
                numOfAnts = 4;
                break;
            }
        }
    }

    public void UpdateSceneToLoad(int _sceneToLoad) {
        sceneToLoad = sceneDropdown.options[_sceneToLoad].text;
    }

    public void UpdateGameMode(int _gamemode) {
        gamemode = gamemodeDropdown.options[_gamemode].text;
    }

    private IEnumerator StartGameCoroutine() {
        LoadingData.gamemode = gamemode;
        LoadingData.sceneToLoad = sceneToLoad;
        LoadingData.playerList = playerList;
        LoadingData.numOfAnts = numOfAnts;

        if (loadingUISpawned == false) {
            loadingUISpawned = true;

            LoadingUI loadingUI = Instantiate(loadingUIPrefab);
            loadingUI.CloseShutter();
        }

        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("LoadingScene");
    }

    //Helper function to return a random color
    private Color ChooseNewColor() {
        int color = Random.Range(1, 8);

        switch (color) {
            case 1:
                return Color.red;
            case 3:
                return Color.black;
            case 4:
                return Color.blue;
            case 5:
                return new Color(0, 0.8f, 0);
            case 6:
                return Color.yellow;
            case 7:
                return Color.cyan;
            case 8:
                return Color.magenta;
            default:
                return Color.red;
        }
    }

    //Called when a user presses the change color button
    public void ChangeColor(int playerNum) {
        PlayerCardInfo playerCard = playerCardList.Where(x => x.playerNum == playerNum).First();

        if (playerCard.isReady == false) {
            Color newColor = ChooseNewColor();
            List<Color> cardColors = new List<Color>();

            foreach (var playerCards in playerCardList) {
                cardColors.Add(playerCards.colorBand.color);
            }

            while (cardColors.Where(x => x == newColor).Count() > 0) {
                newColor = ChooseNewColor();
            }

            if (playerList.Count > 0) {
                playerList.Where(x => x.playerInfo.playerNum == playerNum).First().playerInfo.playerColor = newColor;
            }

            playerCard.colorBand.color = newColor;
        }
    }

    public void ReadyUp(int playerNum) {
        PlayerCardInfo playerCard = playerCardList.Where(x => x.playerNum == playerNum).First();
        playerCard.isReady = !playerCard.isReady;

        if (playerCard.isReady) {
            playerCard.readyText.text = "Ready!";
        }
        else {
            playerCard.readyText.text = "Not Ready!";
        }

        if (playerCardList.All(x => x.isReady) == true) {
            readyCountdownCoroutine = StartCoroutine(ReadyCountdownCoroutine());
        }
        else if (readyCountdownCoroutine != null) {
            StopCoroutine(readyCountdownCoroutine);
            readyCountdownCoroutine = null;
            playersJoinedText.text = "Waiting On Players To Ready Up . . .";
        }
    }

    private IEnumerator ReadyCountdownCoroutine() {
        float time = 3;

        while (time > 0) {
            time -= Time.deltaTime;
            playersJoinedText.text = Mathf.CeilToInt(time).ToString();
            yield return null;
        }

        StartCoroutine(StartGameCoroutine());
    }

    public void ChangeQueenSpecialism(int playerNum) {
        Debug.Log("Not imp.");
    }
}