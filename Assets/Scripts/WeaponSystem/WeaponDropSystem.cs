using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponDropSystem : MonoBehaviour {
    [field : Header("Drop Information")]
    [SerializeField] private float dropOverviewDuration = 2.5f;
    [SerializeField] private WeaponDrop dropPrefab;
    [SerializeField] private List<DropInfo> dropInfo = new List<DropInfo>();

    [Header("Medkit Heal")]
    [SerializeField] private int medkitHealthToHeal;

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
        StartCoroutine(CheckDropCoroutine());
    }

    private IEnumerator CheckDropCoroutine() {
        List<DropInfo> availableDrops = new List<DropInfo>();
        availableDrops = dropInfo.Where(x => x.roundNumber == turnManager.CurrentRound).ToList();

        if (availableDrops.Count() > 0) {
            IsDropping = true;
            cameraSystem.SetCameraTarget(cameraSystem.overviewPosition);
            cameraSystem.CameraDelay(dropOverviewDuration);

            foreach (DropInfo drop in availableDrops) {
                for (int i = 0; i < drop.numOfDrops; i++) {
                    CreateNewDrop(drop.medkitChance);
                }
            }

            yield return new WaitForSeconds(dropOverviewDuration);
            IsDropping = false;
        }
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
public class DropInfo {
    public int roundNumber;
    public int numOfDrops;
    [Range(0, 1)] public float medkitChance;
}

[Serializable]
public class DropChances {
    public BaseWeaponSO weapon;
    [Range(0, 1)] public float dropChance;
}