using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class PlayerCardInfo {
    public int playerNum;
    public GameObject card;
    public TMP_Text playerNumText;
    public TMP_Text joinText;
    public TMP_Text currentInputText;
    public Image colorBand;
}
