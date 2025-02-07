using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsScript : MonoBehaviour
{

	public enum PlayerPrefList {
		Volume,
		Resolution
	}

	public static void SetVolume(float volume) {
		PlayerPrefs.SetFloat("Volume", volume);
	}
	public static float GetVolume() {
		return PlayerPrefs.GetFloat("Volume");
	}
}
