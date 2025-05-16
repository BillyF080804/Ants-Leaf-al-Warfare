using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour {
	[Header("Settings")]
	[SerializeField] private bool playOnAwake = false;
	[SerializeField] private AudioClip audioClip;
	private AudioSource audioSource;

	public enum ObjectType {
		Queen,
		Ant,
		Other
	}

	public ObjectType objectType;

	private void Start() {
		audioSource = GetComponent<AudioSource>();
		switch (objectType) {
			case ObjectType.Queen: {

				break;
			}
			case ObjectType.Ant: {
				break;
			}
			case ObjectType.Other: {
				audioSource.clip = audioClip; 
				break; 
			}
		}


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
