using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuScript : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TMP_Text masterVolumeSliderText;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_Text musicVolumeSliderText;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Text sfxVolumeSliderText;

    private bool fullscreen = true;
    private AudioManager audioManager;

    private void Awake() {
		audioManager = FindFirstObjectByType<AudioManager>();
		Screen.SetResolution(Screen.width, Screen.height, true);
        masterVolumeSlider.value = PlayerPrefs.HasKey("MasterVolume") ? PlayerPrefs.GetFloat("MasterVolume") : 1.0f;
        musicVolumeSlider.value = PlayerPrefs.HasKey("MusicVolume") ? (PlayerPrefs.GetFloat("MusicVolume") * (PlayerPrefs.HasKey("MasterVolume") ? PlayerPrefs.GetFloat("MasterVolume") : 1.0f / 100)) / 100 : 0.5f;
        sfxVolumeSlider.value = PlayerPrefs.HasKey("SFXVolume") ? (PlayerPrefs.GetFloat("SFXVolume") * (PlayerPrefs.HasKey("MasterVolume") ? PlayerPrefs.GetFloat("MasterVolume") : 1.0f / 100)) / 100 : 0.5f;
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

    //Function for changing the master volume
    public void MasterSliderChanged(float value) {
        masterVolumeSliderText.text = value.ToString() + "%";
        PlayerPrefs.SetFloat("MasterVolume", value);

        audioManager.UpdateMasterVolume();
    }

    //Function for changing the music volume
    public void MusicSliderChanged(float value) {
        musicVolumeSliderText.text = value.ToString() + "%";
        PlayerPrefs.SetFloat("MusicVolume", value);

        audioManager.UpdateMusicVolume();
    }

    //Function for changing the SFX volume
    public void SFXSliderChanged(float value) {
        sfxVolumeSliderText.text = value.ToString() + "%";
        PlayerPrefs.SetFloat("SFXVolume", value);

        audioManager.UpdateSFXVolume();
    }

    public float GetVolume() {
        return PlayerPrefs.GetFloat("Volume");
    }
}