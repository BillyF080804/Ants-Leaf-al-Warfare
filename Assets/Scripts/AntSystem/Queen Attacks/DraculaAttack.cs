using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraculaAttack : QueenAttack {

    public EffectScript effect;

    [Tooltip("Use decimal versions of the percentage you want")]
    public List<float> percentages;

    private CameraSystem cameraSystem;

    private void Start() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        audioPlayer = GetComponent<AudioPlayer>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void ActivateAttack(int attackLevel, Ant antInfoScript) {
        StartCoroutine(AttackCoroutine(attackLevel, antInfoScript));
    }

    private IEnumerator AttackCoroutine(int attackLevel, Ant antInfoScript) {
		audioPlayer.PlayClip(audioSource);
		cameraSystem.CameraDelay(5.0f);
        Ant[] antList = FindObjectsOfType<Ant>();
        List<Ant> antList2 = new List<Ant>();
        for (int i = 0; i < antList.Length; i++) {
            if (antList[i].ownedPlayer == antInfoScript.ownedPlayer) {
                antList2.Add(antList[i]);

            }

        }

        int index = attackLevel - 1;
        for (int i = 0; i < antList2.Count; i++) {
            if (attackLevel < percentages.Count) {
                cameraSystem.SetCameraTarget(antList2[i].transform);
                yield return new WaitForSeconds(0.5f);

                effect.AddEffect(antList2[i], percentages[index]);
                yield return new WaitForSeconds(1.0f);
            }

        }
    }

    public override void InitialiseValues(GameObject attackInfo) {
        effect = attackInfo.GetComponent<DraculaAttack>().effect;
        percentages = attackInfo.GetComponent<DraculaAttack>().percentages;

    }
}
