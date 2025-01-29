using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponDropSystem : MonoBehaviour {
    [Header("Drop Information")]
    [SerializeField] private WeaponDrop dropPrefab;
    [SerializeField] private List<DropInfo> dropInfo;

    private TurnManager turnManager;

    private void Start() {
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    public void CheckDrop() {
        List<DropInfo> availableDrops = new List<DropInfo>();
        availableDrops = dropInfo.Where(x => x.roundNumber == turnManager.CurrentRound).ToList();

        if (availableDrops.Count() > 0) {
            foreach (DropInfo drop in availableDrops) {
                for (int i = 0; i < drop.numOfDrops; i++) {
                    CreateNewDrop();
                }
            }
        }
    }

    private void CreateNewDrop() {
        Vector3 randomSpawnPos = new Vector3(Random.Range(-10f, 10f), 10, 0);
        WeaponDrop newDrop = Instantiate(dropPrefab, randomSpawnPos, Quaternion.identity);
        newDrop.SetDropType(GetRandomDrop());
    }

    private string GetRandomDrop() {
        if (Random.value >= 0.75f) {
            return "MedKit";
        }
        else {
            return "Weapon";
        }
    }
}

[Serializable]
public class DropInfo {
    public int roundNumber;
    public int numOfDrops;
}