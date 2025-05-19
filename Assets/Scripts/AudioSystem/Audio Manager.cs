using UnityEngine;

public class AudioManager : MonoBehaviour {
    AudioSource[] sources;

    private void Start() {
        sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        for (int i = 0; i < sources.Length; i++) {
            sources[i].volume = PlayerPrefs.GetFloat("Volume");
        }
    }
}
