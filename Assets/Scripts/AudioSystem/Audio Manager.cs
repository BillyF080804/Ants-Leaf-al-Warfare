using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource), typeof(FadeAudioScript))]
public class AudioManager : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private AudioClip[] musicClips;

    private float masterVolume;
    private float musicVolume;
    private float sfxVolume;

    private FadeAudioScript fadeAudioScript;
    private AudioSource musicSource;
    private List<AudioSource> sfxSources = new List<AudioSource>();

	private void Awake() {
        fadeAudioScript = GetComponent<FadeAudioScript>();
        musicSource = GetComponent<AudioSource>();
        SceneManager.activeSceneChanged += ActiveSceneChanged;

        foreach (AudioSource sfxSource in FindObjectsByType<AudioSource>(FindObjectsSortMode.None)) { 
            if (sfxSource != musicSource) {
                sfxSources.Add(sfxSource); //Gets all non-music audio sources
            }
        }

        //Get previous values or set default values
        masterVolume = PlayerPrefs.HasKey("MasterVolume") ? PlayerPrefs.GetFloat("MasterVolume") : 1.0f;
        musicVolume = PlayerPrefs.HasKey("MusicVolume") ? (PlayerPrefs.GetFloat("MusicVolume") * (masterVolume / 100)) / 100 : 0.5f;
        sfxVolume = PlayerPrefs.HasKey("SFXVolume") ? (PlayerPrefs.GetFloat("SFXVolume") * (masterVolume / 100)) / 100 : 0.5f;

        musicSource.volume = 0;
        if (musicVolume > 0) {
            fadeAudioScript.AudioFade("Open", musicSource, 0.5f, musicVolume); //Update music volume
        }

        if (sfxVolume > 0) {
            foreach (var source in sfxSources) {
                source.volume = sfxVolume; //Update SFX volume
            }
        }

		DontDestroyOnLoad(gameObject);
	}

    //Called when the scene changes
    private void ActiveSceneChanged(Scene currentScene, Scene nextScene) {
        sfxSources.Clear();

        foreach (AudioSource sfxSource in FindObjectsByType<AudioSource>(FindObjectsSortMode.None)) {
            if (sfxSource != musicSource) {
                sfxSources.Add(sfxSource); //Gets all non-music audio sources
            }
        }

        foreach (var sfxAudio in sfxSources) {
            sfxAudio.volume = (sfxVolume * (masterVolume / 100)) / 100; //sets sfx volume
        }

        if (nextScene.name.Contains("Game")) {
            StartCoroutine(SwitchMusicCoroutine(1)); //switch to game music
        }
        else if (!nextScene.name.Contains("Game") && !nextScene.name.Contains("Loading") && musicSource.clip != musicClips[0]) {
            StartCoroutine(SwitchMusicCoroutine(0)); //switch to menu music
        }
    }

    //Function to fade between two music clips
    private IEnumerator SwitchMusicCoroutine(int newMusic) {
        fadeAudioScript.AudioFade("Close", musicSource, 0.5f, musicVolume);
        yield return new WaitUntil(() => musicSource.volume == 0);
        musicSource.clip = musicClips[newMusic];
        musicSource.Play();
        fadeAudioScript.AudioFade("Open", musicSource, 0.5f, musicVolume);
    }

    //Updates all sounds when master volume changed
    public void UpdateMasterVolume() {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");

        musicSource.volume = (musicVolume * (masterVolume / 100)) / 100;

        foreach (var sfxAudio in sfxSources) {
            sfxAudio.volume = (sfxVolume * (masterVolume / 100)) / 100;
        }
    }

    //Updates the music volume when changed
    public void UpdateMusicVolume() {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");

        musicSource.volume = (musicVolume * (masterVolume / 100)) / 100;
    }

    //Updates the SFX volume when changed
    public void UpdateSFXVolume() {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");

        foreach (var sfxAudio in sfxSources) {
            sfxAudio.volume = (sfxVolume * (masterVolume / 100)) / 100;
        }
    }
}
