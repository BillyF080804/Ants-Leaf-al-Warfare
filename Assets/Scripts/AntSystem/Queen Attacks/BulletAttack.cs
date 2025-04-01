using System.Collections.Generic;
using UnityEngine;

public class BulletAttack : QueenAttack {
	[Header("Attack Settings")]
    [SerializeField] private int bulletSpeed = 1;
    [SerializeField] private GameObject bulletPrefab;
	[SerializeField] private BaseWeaponSO weaponType;
	[SerializeField] private List<int> amountOfBulletsPerLevel;

	[Header("Spawning Settings")]
	[SerializeField] private int mapMaxX;
	[SerializeField] private int mapMinX;
	[SerializeField] private int YPos;
	[SerializeField] private int safeRadius;

    public override void ActivateAttack(int attackLevel, Ant antInfoScript, Vector3 position) {
        int amountOfBullets = amountOfBulletsPerLevel[attackLevel - 1];

        for (int i = 0; i < amountOfBullets; i++) {
            Vector2 spawnPos = new Vector2(CheckPos(), YPos);

            GameObject tempBullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            tempBullet.GetComponent<WeaponScript>().SetupWeapon(weaponType, null);
            tempBullet.GetComponent<Rigidbody>().velocity = Vector3.down * bulletSpeed;
        }
    }

	private float CheckPos() {
		float tempX = Random.Range(mapMinX, mapMaxX);
		float maxSafe = transform.position.x + safeRadius;
		float minSafe = transform.position.x - safeRadius;

		while (tempX > minSafe && tempX < maxSafe) {
			tempX = Random.Range(mapMinX, mapMaxX);
		}

		return tempX;
	}
}
