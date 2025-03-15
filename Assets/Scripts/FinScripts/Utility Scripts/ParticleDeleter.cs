using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDeleter : MonoBehaviour
{
    [SerializeField] private float particleLifetime;
    private float timePassed = 0;

    private void Update()
    {
        timePassed = Time.deltaTime;
        if(timePassed >= particleLifetime)
        {
            Destroy(gameObject);
        }
    }

}
