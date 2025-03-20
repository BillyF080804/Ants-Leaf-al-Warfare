using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelControl : MonoBehaviour
{
	public void HidePanel(GameObject panel) {
		panel.SetActive(false);
	}
	public void ShowPanel(GameObject panel) { 
		panel.SetActive(true);
	}
}
