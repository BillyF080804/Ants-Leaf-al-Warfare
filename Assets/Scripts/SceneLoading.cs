using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour {
    private void Start() {
        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine() {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadSceneAsync(LoadingData.sceneToLoad);
    }
}