using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hose : MonoBehaviour {
    [Header("Settings")]
    [SerializeField][Range(0.0f, 1.0f)] private float chancePerTurn = 0.25f;
    [SerializeField] private float explosionForce = 500.0f;
    [SerializeField] private float upwardsModifier = 3;
    [SerializeField] private GameObject waterParticlePrefab = null;
    [SerializeField] private AudioSource audioSource;

    public bool IsSpraying { get; private set; } = false;
    private bool isFirstTurn = true;

    [HideInInspector] public List<Rigidbody> ants = new List<Rigidbody>();
    private CameraSystem cameraSystem;

    private void Start() {
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        cameraSystem = FindFirstObjectByType<CameraSystem>();
    }

    //Dont spray hose at start of first turn
    public void StartHose() {
        if (!isFirstTurn) {
            float value = Random.value; //random spray chance

            if (value < chancePerTurn) {
                StartCoroutine(SprayWaterCoroutine());
            }
        }
        else {
            isFirstTurn = false;
        }
    }

    //Spray water and move ants
    private IEnumerator SprayWaterCoroutine() {
        IsSpraying = true;
        cameraSystem.SetCameraTarget(new Vector3(transform.position.x - 5, transform.position.y, transform.position.z), 5, 10);

        yield return new WaitForSeconds(0.5f);

        GameObject waterParticle = Instantiate(waterParticlePrefab, transform);
        Destroy(waterParticle, 2.5f);

        if (audioSource != null && audioSource.clip != null) {
            audioSource.Play();
        }

        foreach (Rigidbody ant in ants) {
            if (ant != null) {
                ant.AddExplosionForce(explosionForce, new Vector3(ant.transform.position.x + 3.0f, ant.transform.position.y, 0), 10.0f, upwardsModifier, ForceMode.Impulse);
            }
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