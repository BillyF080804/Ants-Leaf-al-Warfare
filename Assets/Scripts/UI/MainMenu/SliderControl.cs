using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour {
    Slider slider;
    [SerializeField] TextMeshProUGUI text;
    public PlayerPrefsScript.PlayerPrefSliderList playerPref;
    private AudioManager audioManager;

    private void Start() {
        audioManager = FindFirstObjectByType<AudioManager>();
        slider = gameObject.GetComponent<Slider>();
        InitialiseValues();
    }

    public void UpdateValue() {
        switch (playerPref) {
            case PlayerPrefsScript.PlayerPrefSliderList.Volume: {
                PlayerPrefsScript.SetVolume(slider.value);
                //audioManager.UpdateVolume();

                text.text = "" + slider.value;
                break;
            }

        }

    }

    public void InitialiseValues() {
        switch (playerPref) {
            case PlayerPrefsScript.PlayerPrefSliderList.Volume: {
                slider.value = PlayerPrefsScript.GetVolume();
                text.text = "" + slider.value;
                break;
            }
        }
    }
}
