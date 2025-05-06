using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FlyTrapHazard: MonoBehaviour
{
    [Header("Variables for Attack")]
    private Ant currentAnt;
    [SerializeField] private int recoveryTurns;
    [SerializeField] private int dazeTurns;
    private List<Ant> antList = new List<Ant>();
    [SerializeField] private GameObject crunchParticles;
    [SerializeField] private int attackDamage;
    [SerializeField] private Transform particleSpawn;
    [SerializeField] private GameObject canvas;

    [Header("Animation Variables")]
    [SerializeField] private string restingBool;
    [SerializeField] string attackTriggerName;
    [SerializeField] string dazeHit;
    [SerializeField] float timeToAttack;
    [SerializeField] private Animator animator;

    [Header("Launch Settings")]
    [SerializeField] private float minPower = 10;
    [SerializeField] private float maxPower = 15;

    private bool chompedThisTurn;
    private int recoveryTurnsPassed = 0;
    private float timePassedForAttack;

    private CameraSystem cameraSystem;

    private void Start() {
        cameraSystem = FindFirstObjectByType<CameraSystem>();   
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<Ant>(out currentAnt);
        if (currentAnt != null)
        {
            canvas.SetActive(true);
            antList.Add(currentAnt);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(antList.Count > 0 && !chompedThisTurn)
        {
            timePassedForAttack += Time.deltaTime;
        }

        if (timePassedForAttack >= timeToAttack)
        {
            chompedThisTurn = true;
            Attack();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent<Ant>(out currentAnt);
        antList.Remove(currentAnt);
        if(antList.Count == 0)
        {
            timePassedForAttack = 0;
            canvas.SetActive(false);
        }
    }

    private void Attack() {
        cameraSystem.SetCameraLookAtTarget(null);
        cameraSystem.SetCameraTarget(transform.position, 0, -10);
        cameraSystem.CameraDelayActive = true;

        foreach(Ant ant in antList) {
            ant.SetCanMove(false);
        }
        canvas.SetActive(false);
        animator.Play(attackTriggerName);
        animator.SetBool(restingBool, true);
        timePassedForAttack = 0;
    }

    public void endOfTurnRecover()
    {
        if(chompedThisTurn)
        {
            recoveryTurnsPassed = recoveryTurnsPassed + 1;
            Debug.Log("TurnsRecovered" + recoveryTurnsPassed);
            if (recoveryTurnsPassed >= recoveryTurns)
            {
                Recovery();
            }
        }
    }

    private void Recovery()
    {
        Debug.Log("Recovered");
        recoveryTurnsPassed = 0;
        animator.SetBool(restingBool, false);
        chompedThisTurn = false;
    }

    public void SpawnParticles()
    {
        if(crunchParticles != null)
        {
            Instantiate(crunchParticles, particleSpawn);
        }
    }

    public void AttackAnts() {
        foreach (Ant ant in antList) {
            float value = 0;

            if (Random.value > 0.5) {
                value = Random.Range(2.0f, 5.0f);
            }
            else {
                value = Random.Range(-5.0f, -2.0f);
            }


            ant.UnFreezeMovement();
            ant.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(minPower, maxPower), new Vector3(ant.transform.position.x + value, ant.transform.position.y, ant.transform.position.z), 5.0f, 3, ForceMode.Impulse);
            ant.TakeDamage(attackDamage);
            ant.SetCanMove(true);
        }

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine() {
        yield return new WaitForSeconds(1.5f);
        cameraSystem.SetCameraTarget(FindFirstObjectByType<TurnManager>().CurrentAntTurn.transform);
        cameraSystem.CameraDelayActive = false;
    }

    public void backToIdle()
    {
        animator.Play("FlyTrapIdle");
    }

    public void GetHit()
    {
        if (!chompedThisTurn)
        {
            chompedThisTurn = true;
            animator.SetTrigger(dazeHit);
            animator.SetBool(restingBool, true);
            recoveryTurnsPassed = dazeTurns;
        }
    }

}
