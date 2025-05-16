using System.Collections;
using UnityEngine;

public class IceAttack : QueenAttack {

	[Header("Attack Settings")]
	[SerializeField] private int iceSpikeSpeed = 1;
	[SerializeField] int attackPerLevel;
	[SerializeField] EffectScript baseSlowEffect;
	[SerializeField] EffectScript maxLevelSlowEffect;
	[SerializeField] private BaseWeaponSO weaponType;
	[SerializeField] int iceSpikeAmount;
	[SerializeField] GameObject iceSpikePrefab;

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
		//audioPlayer.PlayClip();
		int amountOfSpikes = iceSpikeAmount;
		
		cameraSystem.SetCameraLookAtTarget(null);
		cameraSystem.CameraDelay(cameraDelay);

		for (int i = 0; i < amountOfSpikes; i++) {
			Vector2 spawnPos = new Vector2(CheckPos(), YPos);
			cameraSystem.SetCameraTarget(spawnPos, 10, 15);
			yield return new WaitForSeconds(1);

			GameObject tempBullet = Instantiate(iceSpikePrefab, spawnPos, Quaternion.identity);
			if(attackLevel != 3) {
				tempBullet.GetComponent<IceSpikeScript>().InitialiseValues(attackPerLevel, baseSlowEffect);
			} else {
				tempBullet.GetComponent<IceSpikeScript>().InitialiseValues(attackPerLevel, maxLevelSlowEffect);
			}
			
			tempBullet.GetComponent<Rigidbody>().velocity = Vector3.up * iceSpikeSpeed;
			yield return new WaitForSeconds(3);
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
		iceSpikeSpeed = attackInfo.GetComponent<IceAttack>().iceSpikeSpeed;
		attackPerLevel = attackInfo.GetComponent<IceAttack>().attackPerLevel;
		baseSlowEffect = attackInfo.GetComponent<IceAttack>().baseSlowEffect;
		maxLevelSlowEffect = attackInfo.GetComponent<IceAttack>().maxLevelSlowEffect;
		weaponType = attackInfo.GetComponent<IceAttack>().weaponType;
		iceSpikeAmount = attackInfo.GetComponent<IceAttack>().iceSpikeAmount;
		iceSpikePrefab = attackInfo.GetComponent<IceAttack>().iceSpikePrefab;
		YPos = attackInfo.GetComponent<IceAttack>().YPos;
		safeRadius = attackInfo.GetComponent<IceAttack>().safeRadius;
		cameraDelay = attackInfo.GetComponent<IceAttack>().cameraDelay;
	}
}
