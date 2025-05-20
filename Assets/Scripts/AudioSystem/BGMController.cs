using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
	[SerializeField]
	AudioSource audioSource;
	[SerializeField]
	AudioPlayer audioPlayer;

	private void Start() {
		audioPlayer.PlayClip(audioSource);
	}
}
