using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FlyTrapHazard: MonoBehaviour
{
    [Header("Variables for Attack")]
    public Ant currentAnt;
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

    private bool chompedThisTurn;
    private int recoveryTurnsPassed = 0;
    private float timePassedForAttack;


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

    private void Attack()
    {
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
        recoveryTurns = 0;
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

    public void AttackAnts()
    {
        foreach (Ant ant in antList)
        {
            Debug.Log(attackDamage);
            ant.TakeDamage(attackDamage);
        }
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
