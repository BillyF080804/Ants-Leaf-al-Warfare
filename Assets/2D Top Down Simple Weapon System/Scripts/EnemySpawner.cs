using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class EnemySpawner : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxNumOfEnemies;

    private Transform playerTransform;
    private List<GameObject> enemies = new List<GameObject>();

    private void Start() {
        playerTransform = FindFirstObjectByType<PlayerMovementScript>().transform;

        StartCoroutine(EnemySpawnerCoroutine());
    }

    private IEnumerator EnemySpawnerCoroutine() {
        while (true) {
            if (enemies.Count < maxNumOfEnemies) {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.transform.position = new Vector3(playerTransform.position.x + Random.Range(-25f, 25f), playerTransform.position.y + Random.Range(-25f, 25f), playerTransform.position.z);

                if (Vector3.Distance(playerTransform.position, enemy.transform.position) < 7.5f) {
                    Destroy(enemy);
                }
                else {
                    enemies.Add(enemy);
                }
            }

            yield return null;
        }
    }

    public void RemoveEnemy(GameObject enemy) {
        enemies.Remove(enemy);
    }
}