using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hose : MonoBehaviour {
    [Header("Settings")]
    [SerializeField][Range(0.0f, 1.0f)] private float chancePerTurn = 0.25f;
    [SerializeField] private float explosionForce = 250.0f;
    [SerializeField] private GameObject waterParticlePrefab = null;

    public bool IsSpraying { get; private set; } = false;
    private bool isFirstTurn = true;

    [HideInInspector] public List<Rigidbody> ants = new List<Rigidbody>();
    private CameraSystem cameraSystem;

    private void Start() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
    }

    public void StartHose() {
        if (!isFirstTurn) {
            float value = Random.value;

            if (value < chancePerTurn) {
                StartCoroutine(SprayWaterCoroutine());
            }
        }
        else {
            isFirstTurn = false;
        }
    }

    private IEnumerator SprayWaterCoroutine() {
        IsSpraying = true;
        cameraSystem.SetCameraTarget(transform.position, 5, 10);

        GameObject waterParticle = Instantiate(waterParticlePrefab, transform);
        Destroy(waterParticle, 2.5f);

        foreach (Rigidbody ant in ants) {
            ant.AddExplosionForce(explosionForce, new Vector3(ant.transform.position.x + 3.0f, ant.transform.position.y, 0), 10.0f, 1);
        }

        yield return new WaitForSeconds(2.5f);
        IsSpraying = false;
    }

    public void AddAnt(Rigidbody rb) {
        ants.Add(rb);
    }

    public void RemoveAnt(Rigidbody rb) { 
        ants.Remove(rb);
    }
}