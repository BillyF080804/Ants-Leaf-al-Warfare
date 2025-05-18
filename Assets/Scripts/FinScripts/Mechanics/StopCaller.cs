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
        if (partSys != null)
        {
            partSys.Stop();
        }

    }
}
