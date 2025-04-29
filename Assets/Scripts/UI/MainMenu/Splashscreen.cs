using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splashscreen : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private GameObject blackscreen;

    private void Start() {
        StartCoroutine(SplashscreenCoroutine());
    }

    private IEnumerator SplashscreenCoroutine() {
        GetComponent<FadeScript>().FadeOutAndInUI(1.0f, 2.5f, blackscreen);
        yield return null;
        yield return new WaitUntil(() => blackscreen.GetComponent<CanvasGroup>().alpha == 1);
        SceneManager.LoadScene("MainMenu");
    }
}