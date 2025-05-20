using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OptionsMenuScript : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Text volumeText;
	private AudioManager audioManager;
    [SerializeField]
    private Slider volumeSlider;

	private bool fullscreen = true;

    private void Awake() {
		audioManager = FindFirstObjectByType<AudioManager>();
		Screen.SetResolution(Screen.width, Screen.height, true);
        InitaliseVolume();



	}

    public void ChangeFullscreen(bool _fullscreen) {
        fullscreen = _fullscreen;

        if (_fullscreen) {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    public void ChangeResolution(int selectedOption) {
        int xIndex = resolutionDropdown.options[selectedOption].text.IndexOf("x");
        string width = resolutionDropdown.options[selectedOption].text.Substring(0, xIndex);
        string height = resolutionDropdown.options[selectedOption].text.Remove(0, xIndex + 1);

        Screen.SetResolution(int.Parse(width), int.Parse(height), fullscreen);
    }

    public void SetVolume(float volume) {
        PlayerPrefs.SetFloat("Volume", volume);
		audioManager.UpdateVolume();
		volumeText.text = (volume * 100).ToString("F0") + "%";
    }

    public void InitaliseVolume() {
		volumeSlider.value = PlayerPrefsScript.GetVolume();
		volumeText.text = (volumeSlider.value * 100).ToString("F0") + "%";
	}

    public float GetVolume() {
        return PlayerPrefs.GetFloat("Volume");
    }
}