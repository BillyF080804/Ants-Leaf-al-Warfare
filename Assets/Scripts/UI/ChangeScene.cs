using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
	
	public void ChangeCurrentScene(string sceneName) {
		SceneManager.LoadScene(sceneName);
	}

	public void ChangeCurrentScene(int sceneIndex) {
		SceneManager.LoadScene(sceneIndex);
	}
}
