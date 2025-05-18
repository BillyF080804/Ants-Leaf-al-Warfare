using UnityEngine;

public class PlayerPrefsScript : MonoBehaviour
{

	public enum PlayerPrefSliderList {
		Volume,
		
	}
	public enum PlayerPrefDropdownList {
		Resolution,
	}

	public static void SetVolume(float volume) {
		PlayerPrefs.SetFloat("Volume", volume);
	}
	public static float GetVolume() {
		return PlayerPrefs.GetFloat("Volume");
	}

	public static void SetResolution(string resolution) {
		PlayerPrefs.SetString("Resolution", resolution);
	}
	public static string GetResolution() {
		return PlayerPrefs.GetString("Resolution");
	}
}
