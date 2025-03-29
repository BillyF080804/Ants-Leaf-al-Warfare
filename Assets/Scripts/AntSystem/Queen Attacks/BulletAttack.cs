using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttack : QueenAttack {
	[SerializeField]
	GameObject bullet;
	[SerializeField]
	BaseWeaponSO weaponType;

	[SerializeField]
	List<int> amountOfBulletsPerLevel;
	[SerializeField]
	int mapMaxX;
	[SerializeField]
	int mapMinX;
	[SerializeField]
	int YPos;
	[SerializeField]
	int safeRadius;

	public override void ActivateAttack(int attackLevel, Ant antInfoScript, Vector3 position) {
		int amountOfBullets = amountOfBulletsPerLevel[attackLevel-1];
		for(int i = 0; i < amountOfBullets; i++) {
			Vector2 spawnPos = new Vector2(CheckPos(), YPos);
			GameObject tempBullet = Instantiate(bullet, spawnPos, Quaternion.identity);
			tempBullet.GetComponent<WeaponScript>().SetupWeapon(weaponType, null);
		}
	}

	float CheckPos() {
		float tempX = Random.Range(mapMinX,mapMaxX);
		float maxSafe = transform.position.x + safeRadius;
		float minSafe = transform.position.x - safeRadius;
		while (tempX > minSafe && tempX < maxSafe) {
			tempX = Random.Range(mapMinX, mapMaxX);
		}
		return tempX;
	}
}
