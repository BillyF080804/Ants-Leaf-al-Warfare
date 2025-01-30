using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private PlayerInputManager inputManager;

    [Header("Player Cards")]
    [SerializeField] private Transform playerCardHolder;
    [SerializeField] private GameObject playerCard;

    [Header("Other UI")]
    [SerializeField] private GameObject errorText;

    [Header("Scene Info")]
    [SerializeField] private List<SceneAsset> availableScenes;
    [SerializeField] private TMP_Dropdown sceneDropdown;

    private string sceneToLoad;
    private int expectedPlayerCount = 0;
    private int numOfAnts = 2;
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
    }

    //Called when a player changes the number of players in the game
    public void UpdatePlayerCount(int value) {
        if (value == 0) {
            expectedPlayerCount = 2;
        }
        else if (value == 1) {
            expectedPlayerCount = 4;
        }

        foreach (var player in playerCardList) { 
            Destroy(player.card);
        }

        foreach (var player in playerList) {
            Destroy(player.gameObject);
        }

        playerCardList.Clear();
        playerList.Clear();

        for (int i = 0; i < expectedPlayerCount; i++) { 
            GameObject newCard = Instantiate(playerCard, playerCardHolder);
            newCard.name = "PlayerCard" + (i + 1);

            playerCardList.Add(new PlayerCardInfo {
                playerNum = i + 1,
                card = newCard,
                playerNumText = newCard.transform.GetChild(0).GetComponent<TMP_Text>(),
                joinText = newCard.transform.GetChild(1).GetComponent<TMP_Text>(),
                currentInputText = newCard.transform.GetChild(2).GetComponent<TMP_Text>(),
                colorBand = newCard.transform.GetChild(3).GetComponent<Image>()
            });

            playerCardList[i].playerNumText.text = "Player " + (i + 1);

            if (i != 0) {
                while (playerCardList[i].colorBand.color == playerCardList[i - 1].colorBand.color) {
                    playerCardList[i].colorBand.color = ChooseNewColor();
                }    
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

            playerList[playerNum - 1].playerInfo.playerNum = playerNum;
            playerList.Where(x => x.playerInfo.playerNum == playerNum).First().playerInfo.playerInput = input;
            playerList.Where(x => x.playerInfo.playerNum == playerNum).First().playerInfo.playerColor = playerCardList.Where(x => x.playerNum == playerNum).First().colorBand.color;
            playerCardList.Where(x => x.playerNum == playerNum).First().joinText.text = "Player Joined";
            playerCardList.Where(x => x.playerNum == playerNum).First().currentInputText.text = input.devices.First().displayName;
        }
    }

    public void UpdateAntCount(int _numOfAnts) {
        numOfAnts = _numOfAnts;
    }

    public void UpdateSceneToLoad(int _sceneToLoad) {
        sceneToLoad = sceneDropdown.options[_sceneToLoad].text;
    }

    //Loads players into the game
    public void StartGame() {
        if (playerList.Count() != expectedPlayerCount) {
            errorText.SetActive(true);
            errorText.GetComponent<MoveUI>().StartMoveUI(LerpType.OutBack, errorText, new Vector2(0, -25), new Vector2(0, 50), 1.0f);
        }
        else {
            errorText.SetActive(false);
            LoadingData.sceneToLoad = sceneToLoad;
            LoadingData.playerList = playerList;
            LoadingData.numOfAnts = numOfAnts;
            SceneManager.LoadScene("LoadingScene");
        }
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
                return Color.green;
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
        Color newColor = ChooseNewColor();
        List<Color> cardColors = new List<Color>();

        foreach (var playerCard in playerCardList) {
            cardColors.Add(playerCard.colorBand.color);
        }

        while (cardColors.Where(x => x == newColor).Count() > 0) {
            newColor = ChooseNewColor();
        }

        playerList.Where(x => x.playerInfo.playerNum == playerNum).First().playerInfo.playerColor = newColor;
        playerCardList.Where(x => x.playerNum == playerNum).First().colorBand.color = newColor;
    }
}