using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
	[SerializeField] AudioClip audioClip;
	[SerializeField] AudioSource audioSource;

	private void Start() {
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = audioClip;
	}

	public void PlayClip() {
		audioSource.Play();
	}

	public void StopClip() { audioSource.Stop(); }

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
