using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageVolume : MonoBehaviour
{
    [SerializeField] private int damageTaken;
    [SerializeField] private bool causesEffect;
    [SerializeField] private EffectScript effectCause;

    private void OnTriggerEnter(Collider other)
    {
        AntScript ant = other.GetComponent<AntScript>();
        if(ant.GetComponent<AntScript>() != null)
        {
            ant.TakeDamage(damageTaken);
            Debug.Log("DamagedAnt");
            if(causesEffect)
            {
                ant.effects.Add(effectCause);
                Debug.Log(ant.effects.ToString());
            }    
        }    
    }


}
