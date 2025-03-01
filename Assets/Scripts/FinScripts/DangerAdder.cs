using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerAdder : MonoBehaviour
{
    private BarbecueScript bScript;
    private List<AntScript> antsInDanger = new List<AntScript>();
    private AntScript currentAnt;
    private void Awake()
    {
        bScript = GetComponentInParent<BarbecueScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        currentAnt = other.GetComponent<AntScript>();
        if (currentAnt != null) 
        {
            antsInDanger.Add(currentAnt);
        }
        currentAnt = null;
    }

    private void OnTriggerExit(Collider other)
    {
        currentAnt = other.GetComponent<AntScript>();
        if (currentAnt != null)
        {
            antsInDanger.Remove(currentAnt);
        }
        currentAnt = null;
    }

    public void MoveAnts()
    {
        foreach(var ant in antsInDanger)
        {
            bScript.Fall(ant);
        }
    }
}
