using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour
{
	Slider slider;
	[SerializeField] TextMeshProUGUI text;
	public PlayerPrefsScript.PlayerPrefList playerPref;

	private void Start() {
		slider = gameObject.GetComponent<Slider>();
		InitialiseValues();
	}

	public void UpdateValue() {
		switch (playerPref) {
			case PlayerPrefsScript.PlayerPrefList.Volume: {
				PlayerPrefsScript.SetVolume(slider.value);
				text.text = "" + slider.value;
				break;
			}

		}

	}

	public void InitialiseValues() {
		switch(playerPref) {
			case PlayerPrefsScript.PlayerPrefList.Volume: {
				slider.value = PlayerPrefsScript.GetVolume();
				text.text = ""+slider.value;
				break;
			}
		}
	}
}
