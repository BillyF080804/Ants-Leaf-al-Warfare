using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PharaohAttack : QueenAttack {
	[Header("Mummy Prefab")]
	[SerializeField] private GameObject mummySummon;

	[Header("Mummy Settings")]
    [SerializeField] private EffectScript mummyEffect;
    [SerializeField] private int baseHealth = 50;
	[SerializeField] private int healthPerLevel = 15;

	private CameraSystem cameraSystem;

    private void Start() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
    }

    public override void ActivateAttack(int attackLevel, Ant antInfoScript, Vector3 position, TurnManager turnManager) {
		//audioPlayer.PlayClip();
		cameraSystem.SetCameraTarget(position, 5, 10);
		cameraSystem.CameraDelay(3.0f);

		if (attackLevel != 3 && attackLevel > 0) {
			for (int i = 0; i < 2; i++) {
				GameObject tempMummy = Instantiate(mummySummon, position + new Vector3(Random.Range(-5,5),5,0), Quaternion.identity);
				tempMummy.GetComponent<MummyScript>().SetHealth(baseHealth + healthPerLevel * attackLevel);
				tempMummy.GetComponent<MummyScript>().InitialiseMummy(mummyEffect,antInfoScript.ownedPlayer);
				turnManager.PlayerList[(int)antInfoScript.ownedPlayer].AddNewAnt(tempMummy);
			}
		} else {
			for (int i = 0; i < 3; i++) {
				GameObject tempMummy = Instantiate(mummySummon, position + new Vector3(Random.Range(-5, 5), 5, 0), Quaternion.identity);
				tempMummy.GetComponent<MummyScript>().SetHealth(baseHealth + healthPerLevel * 2);
				tempMummy.GetComponent<MummyScript>().InitialiseMummy(mummyEffect, antInfoScript.ownedPlayer);
				turnManager.PlayerList[(int)antInfoScript.ownedPlayer].AddNewAnt(tempMummy);
			}
		}
	}

	public override void InitialiseValues(GameObject attackInfo) {
		mummySummon = attackInfo.GetComponent<PharaohAttack>().mummySummon;
		mummyEffect = attackInfo.GetComponent<PharaohAttack>().mummyEffect;
		baseHealth = attackInfo.GetComponent<PharaohAttack>().baseHealth;
		healthPerLevel = attackInfo.GetComponent<PharaohAttack>().healthPerLevel;
	}
}
