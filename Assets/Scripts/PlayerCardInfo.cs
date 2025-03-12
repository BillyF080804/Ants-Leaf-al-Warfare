using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerCardInfo : MonoBehaviour {
    public int playerNum;
    public bool isReady;
    public GameObject card;
    public TMP_Text playerNumText;
    public TMP_Text joinText;
    public Image colorBand;
    public Image readyBackground;
    public TMP_Text readyText;
    public GameObject readyUpHint;
}
