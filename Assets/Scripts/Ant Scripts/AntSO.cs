using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ant System/New Ant Type", fileName = "New Ant")]
public class AntSO : ScriptableObject {
	[Header("Ant Cosmetics")]
	public GameObject antPrefab = null;
	public Color antColor = Color.black;

	[Header("Ant Attributes")]
	public string antName = "";
	public int moveSpeed = 1;
	public int jumpHeight = 2;
	public int health = 100;

}
