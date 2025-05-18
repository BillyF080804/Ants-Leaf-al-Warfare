using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private TMP_Text winText;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private FadeScript blackscreenFadeScript;
    public static int winningPlayerNumber;

	private void Start() {
        blackscreenFadeScript.FadeOutUI(1.0f);

        foreach (Player player in FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            Destroy(player.gameObject);
        }

        EventSystem.current.SetSelectedGameObject(mainMenuButton);
		SetWinText();
	}

	public void ReturnToMainMenu() {
        StartCoroutine(ReturnToMainMenuCoroutine());
    }

    private IEnumerator ReturnToMainMenuCoroutine() {
        blackscreenFadeScript.gameObject.SetActive(true);
        blackscreenFadeScript.FadeInUI(1.0f);

        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadScene("MainMenu");
    }

    public void SetWinText() {
        winText.text = "Player " + winningPlayerNumber + " wins!";
	}
}