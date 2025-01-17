using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ant System/New Ant Type", fileName = "New Ant")]
public class BaseAntSO : ScriptableObject {
	[Header("Ant Cosmetics")]
	public GameObject antPrefab = null;

	[Header("Ant Attributes")]
	public string antName = "";
	public int moveSpeed = 1;
	public int jumpHeight = 2;
	public int health = 100;
	
}
