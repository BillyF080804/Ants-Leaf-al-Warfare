using System.Collections.Generic;
using UnityEngine;

public class DangerAdder : MonoBehaviour
{
    private BarbecueScript bScript;
    private List<Ant> antsInDanger = new List<Ant>();
    private Ant currentAnt;
    private void Awake()
    {
        bScript = GetComponentInParent<BarbecueScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        currentAnt = other.GetComponent<Ant>();
        if (currentAnt != null) 
        {
            antsInDanger.Add(currentAnt);
        }
        currentAnt = null;
    }

    private void OnTriggerExit(Collider other)
    {
        currentAnt = other.GetComponent<Ant>();
        if (currentAnt != null)
        {
            antsInDanger.Remove(currentAnt);
        }
        currentAnt = null;
    }

    public void MoveAnts()
    {
        foreach(Ant ant in antsInDanger)
        {
            bScript.Fall(ant);
        }
    }
}
