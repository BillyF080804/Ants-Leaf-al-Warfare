using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour {
	[Header("Settings")]
	[SerializeField] private bool playOnAwake = false;
	[SerializeField] private AudioClip audioClip;
	private AudioSource audioSource;

	private void Start() {
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = audioClip;

        audioSource.playOnAwake = playOnAwake;

    }

	public void PlayClip() {
		audioSource.Play();
	}

	public void StopClip() { 
		audioSource.Stop(); 
	}

	public void PauseClip(bool pause) {
		if (pause) {
			audioSource.Pause();
		} else {
			audioSource.UnPause();
		}
	}

	public void ChangeClip(AudioClip newClip) {
		audioClip = newClip;
		audioSource.clip = audioClip;
	}
}
