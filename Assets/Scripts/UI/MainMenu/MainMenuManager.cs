using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private FadeScript blackscreenFadeScript;

    private void Start() {
        blackscreenFadeScript.FadeOutUI(1.0f);
    }
}