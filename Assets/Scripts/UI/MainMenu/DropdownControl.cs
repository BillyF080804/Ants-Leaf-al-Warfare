using TMPro;
using UnityEngine;



public class DropdownControl : MonoBehaviour
{
	TMP_Dropdown dropdown;
	public PlayerPrefsScript.PlayerPrefDropdownList playerPref;

	private void Start() {
		dropdown = gameObject.GetComponent<TMP_Dropdown>();
		InitialiseValues();
	}

	public void UpdateValue() {
		switch (playerPref) {
			case PlayerPrefsScript.PlayerPrefDropdownList.Resolution: {
				PlayerPrefsScript.SetResolution(dropdown.options[dropdown.value].text);
				break;
			}

		}

	}

	public void InitialiseValues() {
		switch (playerPref) {
			case PlayerPrefsScript.PlayerPrefDropdownList.Resolution: {
				string valueToSet = PlayerPrefsScript.GetResolution();
				Debug.Log(valueToSet);
				for(int i = 0; i < dropdown.options.Count; i++) {
					if(valueToSet == dropdown.options[i].text) {
						dropdown.value = i;
					}
				}
				break;
			}
		}
	}
}
