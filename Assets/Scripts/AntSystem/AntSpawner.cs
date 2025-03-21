using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AntSpawner : MonoBehaviour {
    [Header("Spawn Settings")]
    [SerializeField] private float minDistanceBetweenAnts = 3.0f;
    [SerializeField] private List<SpawnArea> spawnAreas = new List<SpawnArea>();
    [SerializeField] private List<Vector3> additionalSpawnPoints = new List<Vector3>();

    [Serializable]
    public class SpawnArea {
        public int minX = 0;
        public int maxX = 0;
    }

    public GameObject SpawnAnt(GameObject prefabToSpawn) {
        GameObject newAnt = Instantiate(prefabToSpawn, GetAntSpawnPoint(), Quaternion.identity);
        return newAnt;
    }

    private Vector3 GetAntSpawnPoint() {
        bool validSpawn = false;
        Vector3 spawnPos = Vector3.zero;
        int spawnAttempts = 0;

        while (validSpawn == false) {
            spawnAttempts++;
            List<Vector3> potentialSpawns = new List<Vector3>();
            spawnPos = Vector3.zero;

            foreach (SpawnArea spawnArea in spawnAreas) {
                Vector3 potentialSpawn = new Vector3(Random.Range(spawnArea.minX, spawnArea.maxX), 30.0f, 0.0f);
                potentialSpawns.Add(potentialSpawn);
            }

            foreach (Vector3 spawn in additionalSpawnPoints) {
                potentialSpawns.Add(spawn);
            }

            spawnPos = potentialSpawns[Random.Range(0, potentialSpawns.Count)];

            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit ray, 35.0f)) {
                spawnPos = new Vector3(ray.point.x, ray.point.y + 0.5f, 0);
            }

            Collider[] colliders = Physics.OverlapSphere(spawnPos, minDistanceBetweenAnts).Where(x => x.CompareTag("Player")).ToArray();

            if (colliders.Count() == 0) {
                validSpawn = true;
            }
            else if (spawnAttempts == 10) {
                Debug.LogError("ERROR: 10 Attempts to spawn ant. Ant Spawning Failed. Spawning at most recent attempt.\nThis can be avoided by either decreasing the distance between spawns or increasing map size.");
                validSpawn = true;
            }
        }

        return spawnPos;
    }
}