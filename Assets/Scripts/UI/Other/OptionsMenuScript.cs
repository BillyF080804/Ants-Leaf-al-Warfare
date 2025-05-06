using TMPro;
using UnityEngine;

public class OptionsMenuScript : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Text volumeText;

    private bool fullscreen = true;

    private void Awake() {
        Screen.SetResolution(Screen.width, Screen.height, true);
        SetVolume(0.5f);
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
        volumeText.text = (volume * 100).ToString("F0") + "%";
    }

    public float GetVolume() {
        return PlayerPrefs.GetFloat("Volume");
    }
}