using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCaller : MonoBehaviour
{
    private ParticleSystem partSys;

    private void Awake()
    {
        partSys = GetComponentInChildren<ParticleSystem>(); 
    }

    public void StopParts()
    {
        partSys.Stop();
    }
}
