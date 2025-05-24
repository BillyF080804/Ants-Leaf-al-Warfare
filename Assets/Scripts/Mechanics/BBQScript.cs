using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBQScript : MonoBehaviour {
    [Header("Collapse Settings")]
    [SerializeField] private int health = 50;
    [SerializeField] private GameObject collapseParticles;

    [Header("Burner Settings")]
    [SerializeField][Range(0.0f, 1.0f)] private float burnChancePerTurn = 0.25f;
    [SerializeField] private int burnDamage = 10;
    [SerializeField] private GameObject fireParticlePrefab;
    [SerializeField] private List<Transform> burnTransforms = new List<Transform>();

    [Header("Audio Settings")]
    [SerializeField] private AudioClip collapseSound;
    [SerializeField] private AudioClip burnSound;

    public bool IsBurning { get; private set; } = false;
    private bool hasCollapsed = false;
    private List<Ant> ants = new List<Ant>();
    private List<Ant> burnerAnts = new List<Ant>();

    private CameraSystem cameraSystem;
    private AudioPlayer audioPlayer;
    private Animator animator;

    private void Start() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        audioPlayer = GetComponent<AudioPlayer>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.TryGetComponent(out WeaponScript weaponScript) && hasCollapsed == false) {
            TakeDamage(weaponScript.weaponInfo.baseDamage);

            if (weaponScript.weaponInfo.hasVFX == true && weaponScript.weaponInfo.vfxObject != null) {
                weaponScript.CreateVFX();
            }

            cameraSystem.StartCameraShake(0.5f, 1.0f);
        }
        else if (collision.gameObject.TryGetComponent(out Ant antScript) && hasCollapsed == true) {
            antScript.TakeDamage(1000);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            ants.Add(other.GetComponent<Ant>());
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            ants.Remove(other.GetComponent<Ant>());
        }
    }

    private void TakeDamage(int damageToDeal) {
        health -= damageToDeal;

        if (health <= 0) {
            CollapseBBQ();
        }
    }

    private void CollapseBBQ() {
        hasCollapsed = true;
        cameraSystem.StartCameraShake(3.0f, 3.0f);
        cameraSystem.SetCameraTarget(transform.position, 5.0f, 30.0f);
        cameraSystem.CameraDelay(4.0f);

        audioPlayer.ChangeClip(collapseSound);
        audioPlayer.PlayClip();
        animator.SetTrigger("CollapseBBQ");

        foreach (Ant ant in ants) {
            ant.isCrushed = true;
            ant.TakeDamage(1000);
        }
    }

    public void BurnChance() {
        float value = Random.value;

        if (value < burnChancePerTurn && hasCollapsed) {
            StartCoroutine(IgniteBurnersCoroutine());
        }
    }

    public void AddBurnerAnt(Ant ant) {
        burnerAnts.Add(ant);
    }

    public void RemoveBurnerAnt(Ant ant) {
        burnerAnts.Remove(ant);
    }

    private IEnumerator IgniteBurnersCoroutine() {
        IsBurning = true;
        cameraSystem.StartCameraShake(0.5f, 1.0f);
        cameraSystem.SetCameraTarget(burnTransforms[1].position, 5.0f, 30.0f);
        audioPlayer.ChangeClip(burnSound);
        audioPlayer.PlayClip();

        foreach (Transform transform in burnTransforms) {
            GameObject fire = Instantiate(fireParticlePrefab, transform);
            fire.transform.localPosition = new Vector3(0, -60, -375);
            fire.transform.localRotation = Quaternion.Euler(90, 0, 0);
            fire.transform.localScale = new Vector3(10, 10, 10);

            Destroy(fire, 3.0f);
        }

        foreach (Ant ant in burnerAnts) {
            ant.TakeDamage(burnDamage);
        }

        yield return new WaitForSeconds(3.0f);
        IsBurning = false;
    }
}