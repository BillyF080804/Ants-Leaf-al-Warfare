using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoseScript : Interactable
{
    private Ant currentAnt;
    bool hasLaunchedThisTurn = false;
    private TurnManager turnM;
    
    [Header("Assignments")]
    [SerializeField] private GameObject waterDrip;
    [SerializeField] private Transform dripPoint;
    [SerializeField] AnimationHandler animHandler;
    [SerializeField] ParticleSystem waterSpray;

    [Header("Animation Variables")]
    [SerializeField] AnimationHandler uiHandler;
    [SerializeField] private string animName;
    [SerializeField] string enterTriggerName;
    [SerializeField] string exitTriggerName;
    [SerializeField] Image promptImage;

    public float imageDistance;

    private void Awake()
    {
        turnM = FindObjectOfType<TurnManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Ant>(out currentAnt) && !hasLaunchedThisTurn)
        {
            if(currentAnt = turnM.CurrentAntTurn)
            {
                uiHandler.ToggleTrigger(enterTriggerName);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(!hasLaunchedThisTurn && imageDistance >= 1.5)
        {
            imageDistance = Vector2.Distance(promptImage.transform.position, currentAnt.transform.position);
            promptImage.transform.position = Vector2.MoveTowards(promptImage.transform.position, currentAnt.transform.position, 1 * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Ant>() == currentAnt)
        {
            uiHandler.ToggleTrigger(exitTriggerName);
            currentAnt = null;
        }

    }

    public override void Interaction()
    {
        if (!hasLaunchedThisTurn)
        {
            WaterSpray();
        }
    }

    public void HoseReset()
    {
        hasLaunchedThisTurn = false;
        if (waterDrip != null)
        {
            waterDrip.SetActive(true);
        }
    }

    private void WaterSpray()
    {
        uiHandler.ToggleTrigger(exitTriggerName);
        waterSpray.Play();
        hasLaunchedThisTurn = true;
        animHandler.ToggleTrigger(animName);
        if(waterDrip != null)
        {
            waterDrip.SetActive(false);
        }
    }
}
