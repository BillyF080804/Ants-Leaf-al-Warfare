using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ant System/New Queen Type", fileName = "New Queen")]
public class BaseQueenAntSO : ScriptableObject {
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
	public string Name;
	public QueenType queenType;
	public int Health = 150;
	public int Damage = 1;
	public int moveSpeed = 1;
	public int jumpHeight = 2;


}
