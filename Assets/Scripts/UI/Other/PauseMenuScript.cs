using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour {
    [Header("Fade Scripts")]
    [SerializeField] private FadeScript mainBackgroundFadeScript;
    [SerializeField] private FadeScript mainPauseMenuFadeScript;
    [SerializeField] private FadeScript blackscreenFadeScript;

    [Header("UI")]
    [SerializeField] private GameObject mainPauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject howToPlayMenu;
    [SerializeField] private GameObject htpInfoPanel;
    [SerializeField] private GameObject htpControlsPanel;
    [SerializeField] private GameObject pauseResumeButton;

    private GameObject panelToHide = null;
    private GameObject panelToShow = null;
    private TurnManager turnManager;

    private void OnEnable() {
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    public void ChangePanels() {
        StartCoroutine(ChangePanelsCoroutine());
    }

    //Switch panels
    private IEnumerator ChangePanelsCoroutine() {
        if (panelToHide == null) {
            Debug.LogError("Please Assign The Panel To Hide.");
        }

        if (panelToShow == null) {
            Debug.LogError("Please Assign The Panel To Show.");
        }

        blackscreenFadeScript.FadeInUI(0.25f);
        yield return new WaitForSeconds(0.25f);
        panelToHide.SetActive(false);
        panelToShow.SetActive(true);
        blackscreenFadeScript.FadeOutUI(0.5f);

        panelToHide = null;
        panelToShow = null;
    }

    public void PanelToHide(GameObject _panelToHide) {
        panelToHide = _panelToHide;
    }

    public void PanelToShow(GameObject _panelToShow) {
        panelToShow = _panelToShow;
    }

    //Display pause menu
    public void DisplayPauseMenu() {
        mainPauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
        howToPlayMenu.SetActive(false);
        htpInfoPanel.SetActive(true);
        htpControlsPanel.SetActive(false);

        SetCurrentSelectedButton(pauseResumeButton);
        mainBackgroundFadeScript.FadeInUI(1.0f);
    }

    public void HidePauseMenu() {
        mainBackgroundFadeScript.FadeOutUI(1.0f);
    }

    //Quit game
    public void QuitGame() {
        foreach (Player player in FindFirstObjectByType<TurnManager>().PlayerList) {
            Destroy(player.gameObject);
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void SetCurrentSelectedButton(GameObject buttonToSelect) {
        turnManager.CurrentPlayerTurn.GetEventSystem().SetSelectedGameObject(buttonToSelect);
    }
}