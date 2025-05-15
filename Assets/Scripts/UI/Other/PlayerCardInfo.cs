using System;
using System.Collections.Generic;
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
    public MoveUI waitingOnPlayerBackground;
    public Image colorBand;
    public TMP_Text readyText;
    public TMP_Text textHint;

    [Header("Queen UI")]
    public Image queenImage;
    public TMP_Text teamText;
	public TextMeshProUGUI queenArchetypeText;
	public TextMeshProUGUI queenDescriptionText;
	public GameObject leftArrow;
    public GameObject rightArrow;

    [Header("Queen Sprites")]
    public List<Sprite> queenSprites = new List<Sprite>();
}
