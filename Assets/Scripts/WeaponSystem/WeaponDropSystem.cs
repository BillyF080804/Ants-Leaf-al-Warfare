using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponDropSystem : MonoBehaviour {
    [Header("Drop Information")]
    [SerializeField] private WeaponDrop dropPrefab;
    [SerializeField] private List<DropInfo> dropInfo = new List<DropInfo>();

    [Header("Drop Chances")]
    [SerializeField] private List<DropChances> dropChances = new List<DropChances>();

    private TurnManager turnManager;

    private void Start() {
        turnManager = FindFirstObjectByType<TurnManager>();

        dropChances = dropChances.OrderBy(x => x.dropChance).ToList();
    }

    public void CheckDrop() {
        List<DropInfo> availableDrops = new List<DropInfo>();
        availableDrops = dropInfo.Where(x => x.roundNumber == turnManager.CurrentRound).ToList();

        if (availableDrops.Count() > 0) {
            foreach (DropInfo drop in availableDrops) {
                for (int i = 0; i < drop.numOfDrops; i++) {
                    CreateNewDrop(drop.medkitChance);
                }
            }
        }
    }

    private void CreateNewDrop(float medkitChance) {
        Vector3 randomSpawnPos = new Vector3(Random.Range(-10f, 10f), 10, 0);
        WeaponDrop newDrop = Instantiate(dropPrefab, randomSpawnPos, Quaternion.identity);
        bool dropIsMedkit = GetRandomDrop(medkitChance);

        if (dropIsMedkit == true) {
            newDrop.SetDropMedkit();
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