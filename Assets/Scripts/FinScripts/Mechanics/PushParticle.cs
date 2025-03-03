using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushParticle : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int pushStrength;
    private AntScript ant;


    private void OnCollisionEnter(Collision collision)
    {
        ant = collision.gameObject.GetComponent<AntScript>();
        if(ant !=null )
        {
            Vector3 contactPos = collision.contacts[0].point;
            Rigidbody antRb = ant.GetComponent<Rigidbody>();
            if( antRb != null )
            {
                antRb.velocity = Vector3.Scale(new Vector3(contactPos.x, contactPos.y, contactPos.z), new Vector3(pushStrength, pushStrength, pushStrength).normalized);
            }
            ant.TakeDamage(damage);
        }
    }
}
