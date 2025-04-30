using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaverAttack : QueenAttack {

	[SerializeField]
	GameObject areaToSpawn;

	[SerializeField]
	EffectScript effectToAdd;

	[SerializeField]
	int mapMinX;
	[SerializeField]
	int mapMaxX;
	[SerializeField]
	int safeRadius = 5;

	public override void ActivateAttack(int attackLevel, Ant antInfoScript, Vector3 position) {
		for (int i = 0; i < attackLevel; i++) {
			Vector3 tempPos = CheckArea();
			GameObject tempArea = Instantiate(areaToSpawn, tempPos, Quaternion.identity);

		}
	}

	public Vector3 CheckArea() {
		Vector3 testArea = FindArea();

		float maxSafe = transform.position.x + safeRadius;
		float minSafe = transform.position.x - safeRadius;
		while (testArea.x > minSafe && testArea.x < maxSafe) {
			testArea = FindArea();
		}
		return testArea;
	}

	public Vector3 FindArea() {
		Vector3 spawnPos = new Vector3();
		if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit ray, 35.0f)) {
			spawnPos = new Vector3(Random.Range(mapMinX, mapMaxX), ray.point.y + 0.5f, 0);
		}
		return spawnPos;
	}

	public override void InitialiseValues(GameObject attackInfo) {
		areaToSpawn = attackInfo.GetComponent<WeaverAttack>().areaToSpawn;
		effectToAdd = attackInfo.GetComponent<WeaverAttack>().effectToAdd;
		mapMinX = attackInfo.GetComponent<WeaverAttack>().mapMinX;
		mapMaxX = attackInfo.GetComponent<WeaverAttack>().mapMaxX;
		safeRadius = attackInfo.GetComponent<WeaverAttack>().safeRadius;
	}
}
