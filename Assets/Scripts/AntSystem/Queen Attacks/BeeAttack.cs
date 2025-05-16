using System.Collections;
using UnityEngine;

public class BeeAttack : QueenAttack {
	[Header("Settings")]
	[SerializeField] private GameObject beeSummon;

	private CameraSystem cameraSystem;

    private void Start() {
		cameraSystem = FindFirstObjectByType<CameraSystem>();
    }

    public override void ActivateAttack(int attackLevel, Ant antInfoScript, Vector3 position) {
		StartCoroutine(AttackCoroutine(attackLevel, antInfoScript, position));
	}

	private IEnumerator AttackCoroutine(int attackLevel, Ant antInfoScript, Vector3 position) {
        //audioPlayer.PlayClip();
        Transform focusObject = null;
        cameraSystem.CameraDelayActive = true;

        for (int i = 0; i < attackLevel; i++) {
            BeeScript beeScript = Instantiate(beeSummon, position, Quaternion.identity).GetComponent<BeeScript>();
            beeScript.InitialiseValues(antInfoScript, attackLevel);
            focusObject = beeScript.transform;
            cameraSystem.SetCameraTarget(focusObject);
        }

        yield return new WaitUntil(() => focusObject == null);
        cameraSystem.CameraDelayActive = false;
    }

	public override void InitialiseValues(GameObject attackInfo) {
		beeSummon = attackInfo.GetComponent<BeeAttack>().beeSummon;
	}
}
