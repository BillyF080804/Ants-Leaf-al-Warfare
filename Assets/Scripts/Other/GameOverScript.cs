using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour {
    [SerializeField]
    TextMeshProUGUI winText;
    public static int winningPlayerNumber;

	private void Start() {
		SetWinText();
	}

	public void ReturnToLobby() {
        SceneManager.LoadScene("LobbyScene");
    }

    public void SetWinText() {
        winText.text = "Player " + winningPlayerNumber + " wins!";

	}
}