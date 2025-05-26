using System.Collections;
using UnityEngine;

public class WeaverAttack : QueenAttack {
    [Header("Settings")]
    [SerializeField] private GameObject areaToSpawn;
    [SerializeField] private EffectScript effectToAdd;
    [SerializeField] private int safeRadius = 5;

    private CameraSystem cameraSystem;
    private TurnManager turnManager;

    private void Start() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        turnManager = FindFirstObjectByType<TurnManager>();
        audioPlayer = GetComponent<AudioPlayer>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void ActivateAttack(int attackLevel, Ant antInfoScript, Vector3 position) {
        StartCoroutine(AttackCoroutine(attackLevel, antInfoScript, position));
    }

    private IEnumerator AttackCoroutine(int attackLevel, Ant antInfoScript, Vector3 position) {
        audioPlayer.PlayClip(audioSource);
		cameraSystem.CameraDelay(attackLevel + 1);

        for (int i = 0; i < attackLevel; i++) {
            Vector3 tempPos = CheckArea();
            GameObject tempArea = Instantiate(areaToSpawn, tempPos, Quaternion.identity);

            cameraSystem.SetCameraTarget(tempArea.transform);
            yield return new WaitForSeconds(1.0f);
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
        Vector3 spawnPos = new Vector3(Random.Range(turnManager.MapMinX, turnManager.MapMaxX), 30, 0);
        if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit ray, 35.0f)) {
            spawnPos = new Vector3(spawnPos.x, ray.point.y + 0.5f, 0);
        }

        return spawnPos;
    }

    public override void InitialiseValues(GameObject attackInfo) {
        areaToSpawn = attackInfo.GetComponent<WeaverAttack>().areaToSpawn;
        effectToAdd = attackInfo.GetComponent<WeaverAttack>().effectToAdd;
        safeRadius = attackInfo.GetComponent<WeaverAttack>().safeRadius;
    }
}
