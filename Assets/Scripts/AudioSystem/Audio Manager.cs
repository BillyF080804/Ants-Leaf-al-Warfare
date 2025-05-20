using UnityEngine;

public class AudioManager : MonoBehaviour {
    AudioSource[] sources;
	[SerializeField]
	AudioClip menuMusic;
	[SerializeField]
	AudioClip gameMusic;



	private void Start() {
        sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        for (int i = 0; i < sources.Length; i++) {
            sources[i].volume = PlayerPrefs.GetFloat("Volume");
        }
		DontDestroyOnLoad(gameObject);
	}

    public void UpdateVolume() {
		sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
		for (int i = 0; i < sources.Length; i++) {
			sources[i].volume = PlayerPrefs.GetFloat("Volume");
		}
	}

	public void SwitchMusic(bool isNextSceneGame) {
		AudioSource tempAudioSource = gameObject.GetComponent<AudioSource>();
		if (isNextSceneGame) {
			tempAudioSource.clip = gameMusic;
		} else {
			tempAudioSource.clip = menuMusic;
		}
		tempAudioSource.Play();


	}
}
