using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponDropSystem : MonoBehaviour {
    [Header("Drop Settings")]
    [SerializeField] private float dropOverviewDuration = 2.5f;
    [SerializeField] private int minNumOfCrates = 1;
    [SerializeField] private int maxNumOfCrates = 3;
    [SerializeField] private int medkitHealthToHeal = 10;
    [SerializeField][Range(0.0f, 1.0f)] private float medkitChance = 0.25f;
    [SerializeField][Range(0.0f, 1.0f)] private float dropChancePerTurn = 0.25f;

    [Header("Drop Prefab")]
    [SerializeField] private WeaponDrop dropPrefab;

    [Header("Drop Chances")]
    [SerializeField] private List<DropChances> dropChances = new List<DropChances>();

    public bool IsDropping { get; private set; } = false;
    private TurnManager turnManager;
    private CameraSystem cameraSystem;

    private void Start() {
        turnManager = FindFirstObjectByType<TurnManager>();
        cameraSystem = FindFirstObjectByType<CameraSystem>();

        if (dropChances.Count == 0) {
            Debug.LogWarning("WeaponDropSystem: No drop chances set up. Please add at least one drop chance.");
        }
        else {
            dropChances = dropChances.OrderBy(x => x.dropChance).ToList();
        }
    }

    public void CheckDrop() {
        float randomNum = Random.value;

        if (randomNum < dropChancePerTurn) {
            StartCoroutine(CheckDropCoroutine());
        }
    }

    private IEnumerator CheckDropCoroutine() {
        IsDropping = true;
        cameraSystem.SetCameraTarget(null);
        cameraSystem.CameraDelay(dropOverviewDuration);

        int randomNum = Random.Range(minNumOfCrates, maxNumOfCrates);

        for (int i = 0; i < randomNum; i++) {
            CreateNewDrop(medkitChance);
        }

        yield return new WaitForSeconds(dropOverviewDuration);
        IsDropping = false;
    }

    private void CreateNewDrop(float medkitChance) {
        Vector3 randomSpawnPos = new Vector3(Random.Range(turnManager.MapMinX, turnManager.MapMaxX), 10, 0);
        WeaponDrop newDrop = Instantiate(dropPrefab, randomSpawnPos, Quaternion.identity);
        bool dropIsMedkit = GetRandomDrop(medkitChance);

        if (dropIsMedkit == true) {
            newDrop.SetDropMedkit(medkitHealthToHeal);
        }
        else {
            newDrop.SetDropWeapon(GetRandomWeapon());
        }
    }

    private bool GetRandomDrop(float medkitChance) {
        if (Random.value <= medkitChance) {
            return true;
        }
        else {
            return false;
        }
    }

    private BaseWeaponSO GetRandomWeapon() {
        foreach (DropChances weaponDrop in dropChances) {
            if (Random.value <= weaponDrop.dropChance) {
                return weaponDrop.weapon;
            }
        }

        return dropChances.Last().weapon;
    }
}

[Serializable]
public class DropChances {
    public BaseWeaponSO weapon;
    [Range(0, 1)] public float dropChance;
}