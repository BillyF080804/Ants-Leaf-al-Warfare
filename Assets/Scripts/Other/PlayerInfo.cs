using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerInfo {
    public int playerNum;
    public string queenType = "Bee";
    public Color playerColor;
    public PlayerInput playerInput;
}
