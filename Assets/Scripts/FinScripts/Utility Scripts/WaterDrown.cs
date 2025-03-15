using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

public class WaterDrown : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] float minDistance;
    [SerializeField] Transform riseSpot;
    [SerializeField] Transform floatAway1;
    [SerializeField] Transform floatAway2;
    [SerializeField] Vector3 fallDistance;
    [SerializeField] float timeBeforeRise;
    [SerializeField] float riseSpeed;
    [SerializeField] GameObject waterRippleParticle;

    float currentDistance;
    Ant antScript;
    Rigidbody currentRb;
    Transform antSform;
    
    bool impacted;
    bool floatedUp;
    bool shiftedOnce;
    bool shiftedTwice;


    private void OnTriggerEnter(Collider other)
    {

        antScript = other.GetComponent<Ant>();
        if ( antScript != null )
        {
            currentRb = antScript.GetComponent<Rigidbody>();
            if ( currentRb != null )
            {
                Instantiate(waterRippleParticle, antScript.transform);
                Debug.Log("ReachedMove");
                currentRb.useGravity = false;
                currentRb.velocity = Vector3.zero;
                currentRb.AddForce(fallDistance, ForceMode.Impulse);
                antSform =antScript.GetComponent<Transform>();
                impacted = true;
            }
        }
    }

    private IEnumerator MoveDelay()
    {
        Debug.Log("Moving");
        yield return new WaitForSeconds(timeBeforeRise);
        currentRb.velocity = Vector3.zero;
        antSform.transform.position = Vector3.Lerp(currentRb.gameObject.transform.position, riseSpot.transform.position, riseSpeed * Time.deltaTime);
        currentDistance = Vector3.Distance(antSform.position, riseSpot.position);
        if (minDistance >= currentDistance)
        {
            impacted = false;
            floatedUp = true;
            StopCoroutine(MoveDelay());
            currentDistance = 0;
        }
    }

    private void FixedUpdate()
    {
        if(antSform != null)
        {
            AntDistanceChecks();
        }
    }

    private void AntDistanceChecks()
    {
        if (impacted)
        {
            StartCoroutine(MoveDelay());

        }
        else if (floatedUp)
        {
            antSform.transform.position = Vector3.Lerp(antSform.transform.position, floatAway1.position, riseSpeed * Time.deltaTime);
            currentDistance = Vector3.Distance(antSform.position, floatAway1.position);
            if (minDistance >= currentDistance)
            {
                floatedUp = false;
                shiftedOnce = true;
                currentDistance = 0;
            }
        }
        else if (shiftedOnce)
        {
            antSform.transform.position = Vector3.Lerp(antSform.transform.position, floatAway2.position, riseSpeed * Time.deltaTime);
            currentDistance = Vector3.Distance(antSform.position, floatAway2.position);
            if (minDistance >= currentDistance)
            {
                shiftedOnce = false;
                shiftedTwice = true;
                currentDistance = 0;
            }
        }

        else if (shiftedTwice)
        {
            antScript.OnDeath();
            Destroy(antSform.gameObject);
            antSform = null;
            currentRb = null;
            antScript = null;
        }
    }
}
