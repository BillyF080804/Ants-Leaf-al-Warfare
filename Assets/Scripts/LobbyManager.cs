using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using static AntSO;

public class LobbyManager : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private PlayerInputManager inputManager;

    [Header("Player Cards")]
    [SerializeField] private List<PlayerCardInfo> playerCards;

    [Header("Other UI")]
    [SerializeField] private TMP_Text playersJoinedText;
    [SerializeField] private TMP_Dropdown gamemodeDropdown;
    [SerializeField] private LoadingUI loadingUIPrefab;

    [Header("Scene Info")]
    [SerializeField] private List<string> availableScenes = new List<string>();
    [SerializeField] private TMP_Dropdown sceneDropdown;

    private string gamemode;
    private string sceneToLoad;
    private int numOfAnts = 2;
    private bool loadingUISpawned = false;

    private Coroutine readyCountdownCoroutine = null;
    private List<Player> playerList = new List<Player>();

    //Used to change the text that shows for the archetype and description for each queen
	[SerializeField]
	List<AntSO> antInformation;


	private void Start() {
        sceneDropdown.AddOptions(availableScenes);

        UpdateSceneToLoad(0);
        UpdateGameMode(0);
    }

    //Called when a player presses the join button on their controller
    public void OnPlayerJoined(PlayerInput input) {
        if (playerList.Count < 4) {
            playerList.Add(input.gameObject.GetComponent<Player>());

            int playerNum = playerList.Count;
            input.gameObject.name = "Player" + playerNum;

            Player player = playerList[playerNum - 1];
            PlayerCardInfo playerCard = playerCards[playerNum - 1];

            playerCard.closeBackground.SetActive(false);
            playerCard.mainBackground.SetActive(true);

			ChangeQueenSpecialismText("Bee", playerCard);

			player.playerInfo.playerNum = playerNum;
            player.playerInfo.playerInput = input;
            playerCard.playerJoined = true;
            player.playerInfo.playerColor = playerCards.Where(x => x.playerNum == playerNum).First().colorBand.color;
            playerCard.readyText.text = input.actions.FindAction("ReadyUp").GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions) + " - Not Ready!";

            if (playerList.Count > 0) {
                playersJoinedText.text = "Waiting On Players To Ready Up . . .";
            }

            if (readyCountdownCoroutine != null) {
                StopCoroutine(readyCountdownCoroutine);
                readyCountdownCoroutine = null;
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

    public void ReadyUp(int playerNum) {
        PlayerCardInfo playerCard = playerCards.Where(x => x.playerNum == playerNum).First();
        Player player = playerList.Where(x => x.playerInfo.playerNum == playerNum).First();
        playerCard.isReady = !playerCard.isReady;

        if (playerCard.isReady) {
            playerCard.readyText.text = player.GetKeybindForAction("ReadyUp") + " - Ready!";
        }
        else {
            playerCard.readyText.text = player.GetKeybindForAction("ReadyUp") + " - Not Ready!";
        }

        if (playerCards.Where(x => x.playerJoined == true).All(x => x.isReady) == true) {
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
            playersJoinedText.text = "Starting Game In . . . " + Mathf.CeilToInt(time).ToString();
            yield return null;
        }

        StartCoroutine(StartGameCoroutine());
    }

    public void ChangeQueenSpecialism(int playerNum, float value) {
        Player player = playerList.Where(x => x.playerInfo.playerNum == playerNum).First();
        PlayerCardInfo playerCard = playerCards.Where(x => x.playerNum == playerNum).First(); 

        if (value > 0 && playerCard.isReady == false) {
            switch (player.playerInfo.queenType) {
                case "Bee":
                    player.playerInfo.queenType = "Dracula";
                    playerCard.teamText.text = "Dracula Ant";
                    playerCard.leftArrow.SetActive(true);
				    ChangeQueenSpecialismText("Dracula", playerCard);
				    break;
                case "Dracula":
                    player.playerInfo.queenType = "Weaver";
                    playerCard.teamText.text = "Weaver Ant";
				    ChangeQueenSpecialismText("Weaver", playerCard);
				    break;
                case "Weaver":
                    player.playerInfo.queenType = "Pharaoh";
                    playerCard.teamText.text = "Pharaoh Ant";
				    ChangeQueenSpecialismText("Pharaoh", playerCard);
				    break;
                case "Pharaoh":
                    player.playerInfo.queenType = "Ice";
                    playerCard.teamText.text = "Ice Ant";
				    ChangeQueenSpecialismText("Ice", playerCard);
				    break;
                case "Ice":
                    player.playerInfo.queenType = "Bullet";
                    playerCard.teamText.text = "Bullet Ant";
                    playerCard.rightArrow.SetActive(false);
				    ChangeQueenSpecialismText("Bullet", playerCard);
				    break;
            }
        }
        else if (value < 0 && playerCard.isReady == false) {
            switch (player.playerInfo.queenType) {
                case "Bullet":
                    player.playerInfo.queenType = "Ice";
                    playerCard.teamText.text = "Ice Ant";
                    playerCard.rightArrow.SetActive(true);
				    ChangeQueenSpecialismText("Ice", playerCard);
				    break;
				case "Ice":
                    player.playerInfo.queenType = "Pharaoh";
                    playerCard.teamText.text = "Pharaoh Ant";
				    ChangeQueenSpecialismText("Pharaoh", playerCard);
				    break;
                case "Pharaoh":
                    player.playerInfo.queenType = "Weaver";
                    playerCard.teamText.text = "Weaver Ant";
				    ChangeQueenSpecialismText("Weaver", playerCard);
				    break;
                case "Weaver":
                    player.playerInfo.queenType = "Dracula";
                    playerCard.teamText.text = "Dracula Ant";
                    ChangeQueenSpecialismText("Dracula", playerCard);
                    break;
                case "Dracula":
                    player.playerInfo.queenType = "Bee";
                    playerCard.teamText.text = "Bee Ant";
                    playerCard.leftArrow.SetActive(false);
                    ChangeQueenSpecialismText("Bee", playerCard);

					break;
            }
        }
    }



    public void ChangeQueenSpecialismText(string queenType, PlayerCardInfo playerCard) {
        AntSO currentAnt = null;
        switch (queenType) {
            case "Bullet": {
                currentAnt = antInformation.Where(x => x.queenType == AntSO.QueenType.Bullet).First();
				playerCard.queenArchetypeText.text = "Type: " + currentAnt.queenArchetype;
				playerCard.queenDescriptionText.text = "Description: "+currentAnt.description;
                break;
            }
            case "Bee": {
                currentAnt = antInformation.Where(x => x.queenType == AntSO.QueenType.Bee).First();
				playerCard.queenArchetypeText.text = "Type: " + currentAnt.queenArchetype;
				playerCard.queenDescriptionText.text = "Description: " + currentAnt.description;
				break;
            }
            case "Weaver": {
                currentAnt = antInformation.Where(x => x.queenType == AntSO.QueenType.Weaver).First();
				playerCard.queenArchetypeText.text = "Type: " + currentAnt.queenArchetype;
				playerCard.queenDescriptionText.text = "Description: " + currentAnt.description;
				break;
            }
            case "Ice": {
                currentAnt = antInformation.Where(x => x.queenType == AntSO.QueenType.Ice).First();
				playerCard.queenArchetypeText.text = "Type: " + currentAnt.queenArchetype;
				playerCard.queenDescriptionText.text = "Description: " + currentAnt.description;
				break;
            }
            case "Dracula": {
                currentAnt = antInformation.Where(x => x.queenType == AntSO.QueenType.Dracula).First();
				playerCard.queenArchetypeText.text = "Type: " + currentAnt.queenArchetype;
				playerCard.queenDescriptionText.text = "Description: " + currentAnt.description;
				break;
            }
            case "Pharaoh": {
                currentAnt = antInformation.Where(x => x.queenType == AntSO.QueenType.Pharaoh).First();
				playerCard.queenArchetypeText.text = "Type: " + currentAnt.queenArchetype;
				playerCard.queenDescriptionText.text = "Description: " + currentAnt.description;
				break;
            }
            default: {
                Debug.LogError("Invalid Queen Ant - " + queenType);
                break;
            }
        }
    }
}