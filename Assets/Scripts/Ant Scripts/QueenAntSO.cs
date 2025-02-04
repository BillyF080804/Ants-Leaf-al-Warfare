using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ant System/New Queen Type", fileName = "New Queen")]
public class QueenAntSO : ScriptableObject {
	public enum QueenType { 
		Fire,
		Bee,
		Pharaoh,
		Weaver,
		Dracula,
		Bullet
	}

	[Header("Ant Cosmetics")]
	public GameObject antPrefab = null;
	public Color antColor = Color.black;

	[Header("Queen Attributes")]
	public string antName;
	public QueenType queenType;
	public int health = 150;
	public int damage = 1;
	public int moveSpeed = 3;
	public int jumpHeight = 2;
}
