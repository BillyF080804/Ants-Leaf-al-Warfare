using System.Collections;
using UnityEngine;

public class WaterDrown : MonoBehaviour {
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

    private bool impacted;
    private bool floatedUp;
    private bool shiftedOnce;
    private bool shiftedTwice;
    private Vector3 oldPos;
    private bool hasSetOldPos = false;


    private void OnTriggerEnter(Collider other) {

        antScript = other.GetComponent<Ant>();
        if (antScript != null) {
            currentRb = antScript.GetComponent<Rigidbody>();
            if (currentRb != null) {
                antScript.SetCanMove(false);
                Instantiate(waterRippleParticle, antScript.transform);
                Debug.Log("ReachedMove");
                currentRb.useGravity = false;
                currentRb.velocity = Vector3.zero;
                currentRb.AddForce(fallDistance, ForceMode.Impulse);
                antSform = antScript.GetComponent<Transform>();
                impacted = true;
            }
        }
    }

    private void FixedUpdate() {
        if (antSform != null) {
            AntDistanceChecks();
        }
    }

    private void AntDistanceChecks() {
        if (impacted) {
            if (!hasSetOldPos) {
                oldPos = antSform.position;
                hasSetOldPos = true;
            }
            StartCoroutine(MoveDelay());

        }
        else if (floatedUp) {
            if (!hasSetOldPos) {
                oldPos = antSform.position;
                hasSetOldPos = true;
            }
            MoveTo1();
        }
        else if (shiftedOnce) {
            if (!hasSetOldPos) {
                oldPos = antSform.position;
                hasSetOldPos = true;
            }
            MoveTo2();
        }

        else if (shiftedTwice) {
            antScript.OnDeath();
            antSform = null;
            currentRb = null;
            antScript = null;
        }
    }

    private void MoveTo1() {
        Debug.Log("Moving one");
        antSform.transform.position = Vector3.Lerp(oldPos, floatAway1.position, riseSpeed);
        currentDistance = Vector3.Distance(antSform.position, floatAway1.position);
        if (minDistance >= currentDistance) {
            floatedUp = false;
            shiftedOnce = true;
            currentDistance = 0;
            hasSetOldPos = false;
        }
    }

    private void MoveTo2() {
        Debug.Log("Moving two");
        antSform.transform.position = Vector3.Lerp(oldPos, floatAway2.position, riseSpeed);
        currentDistance = Vector3.Distance(antSform.position, floatAway2.position);
        if (minDistance >= currentDistance) {
            shiftedOnce = false;
            shiftedTwice = true;
            currentDistance = 0;
            hasSetOldPos = false;
        }
    }



    private IEnumerator MoveDelay() {
        Debug.Log("Moving");
        yield return new WaitForSeconds(timeBeforeRise);
        currentRb.velocity = Vector3.zero;
        antSform.transform.position = Vector3.Lerp(oldPos, riseSpot.transform.position, riseSpeed);
        currentDistance = Vector3.Distance(antSform.position, riseSpot.position);
        if (minDistance >= currentDistance) {
            impacted = false;
            floatedUp = true;
            currentDistance = 0;
            hasSetOldPos = false;
        }
    }
}
