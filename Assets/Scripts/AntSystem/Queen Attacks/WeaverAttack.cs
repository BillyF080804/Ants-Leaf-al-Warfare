using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaverAttack : QueenAttack {

	[SerializeField]
	GameObject areaToSpawn;

	[SerializeField]
	EffectScript effectToAdd;

	public int mapMinX;
	public int mapMaxX;

	public override void ActivateAttack(int attackLevel, Ant antInfoScript, Vector3 position) {
		Debug.Log("hi");
		for (int i = 0; i < attackLevel; i++) {
			GameObject tempArea = Instantiate(areaToSpawn, FindArea(),Quaternion.identity);

		}
	}

	public Vector3 FindArea() {
		Vector3 spawnPos = new Vector3();
		if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit ray, 35.0f)) {
			spawnPos = new Vector3(Random.Range(mapMinX, mapMaxX), ray.point.y + 0.5f, 0);
		}


		return spawnPos;
	}
}
