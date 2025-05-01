using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttack : QueenAttack {
	[Header("Attack Settings")]
    [SerializeField] private int bulletSpeed = 1;
    [SerializeField] private GameObject bulletPrefab;
	[SerializeField] private BaseWeaponSO weaponType;
	[SerializeField] private List<int> amountOfBulletsPerLevel;

	[Header("Spawning Settings")]
	[SerializeField] private int YPos;
	[SerializeField] private int safeRadius;

	[Header("Camera Settings")]
	[SerializeField] private float cameraDelay = 5.0f;

	private CameraSystem cameraSystem;
	private TurnManager turnManager;

    private void Start() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    public override void ActivateAttack(int attackLevel, Ant antInfoScript, Vector3 position) {
        StartCoroutine(AttackCoroutine(attackLevel, antInfoScript, position));
    }

	private IEnumerator AttackCoroutine(int attackLevel, Ant antInfoScript, Vector3 position) {
        int amountOfBullets = amountOfBulletsPerLevel[attackLevel - 1];
        cameraSystem.SetCameraTarget(null);
        cameraSystem.SetCameraLookAtTarget(null);
        cameraSystem.CameraDelay(cameraDelay);

        for (int i = 0; i < amountOfBullets; i++) {
            Vector2 spawnPos = new Vector2(CheckPos(), YPos);

            GameObject tempBullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            tempBullet.GetComponent<WeaponScript>().SetupWeapon(weaponType, null);
            tempBullet.GetComponent<Rigidbody>().velocity = Vector3.down * bulletSpeed;
			yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
    }

	private float CheckPos() {
		float tempX = Random.Range(turnManager.MapMinX, turnManager.MapMaxX);
		float maxSafe = transform.position.x + safeRadius;
		float minSafe = transform.position.x - safeRadius;

		while (tempX > minSafe && tempX < maxSafe) {
			tempX = Random.Range(turnManager.MapMinX, turnManager.MapMaxX);
		}

		return tempX;
	}

	public override void InitialiseValues(GameObject attackInfo) {
		bulletSpeed = attackInfo.GetComponent<BulletAttack>().bulletSpeed;
		bulletPrefab = attackInfo.GetComponent<BulletAttack>().bulletPrefab;
		weaponType = attackInfo.GetComponent<BulletAttack>().weaponType;
		amountOfBulletsPerLevel = attackInfo.GetComponent<BulletAttack>().amountOfBulletsPerLevel;
		YPos = attackInfo.GetComponent<BulletAttack>().YPos;
		safeRadius = attackInfo.GetComponent<BulletAttack>().safeRadius;
	}
}
