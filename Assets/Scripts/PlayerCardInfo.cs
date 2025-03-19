using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerCardInfo : MonoBehaviour {
    [Header("Settings")]
    public int playerNum;
    public bool isReady = false;
    public bool playerJoined = false;

    [Header("UI")]
    public GameObject mainBackground;
    public GameObject closeBackground;
    public Image colorBand;
    public TMP_Text readyText;
    public TMP_Text textHint;

    [Header("Queen UI")]
    public Image queenImage;
    public TMP_Text teamText;
    public GameObject leftArrow;
    public GameObject rightArrow;
}
