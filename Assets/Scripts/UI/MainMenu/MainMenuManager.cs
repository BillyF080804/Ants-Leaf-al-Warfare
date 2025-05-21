using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private FadeScript blackscreenFadeScript;
    [SerializeField] private AudioManager audioManagerPrefab;

    [Header("Loading UI")]
    [SerializeField] private LoadingUI loadingUIPrefab;

    private bool loadingUISpawned = false;
    private GameObject panelToShow = null;
    private GameObject panelToHide = null;

    private void Start() {
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        blackscreenFadeScript.FadeOutUI(1.0f);

        if (audioManager == null) {
            Instantiate(audioManagerPrefab);
        }
    }

    public void PlayGameButton() {
        StartCoroutine(PlayGameButtonCoroutine());
    }

    private IEnumerator PlayGameButtonCoroutine() {
        LoadingData.sceneToLoad = "LobbyScene";

        if (loadingUISpawned == false) {
            loadingUISpawned = true;

            LoadingUI loadingUI = Instantiate(loadingUIPrefab);
            loadingUI.CloseShutter();
        }

        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("LoadingScene");
    }

    public void ChangePanels() {
        StartCoroutine(ChangePanelsCoroutine());
    }

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

    public void QuitGame() {
        Application.Quit();
    }

    public void LoadGameXScene() {
        SceneManager.LoadScene("GameXSecretScene");
    }
}