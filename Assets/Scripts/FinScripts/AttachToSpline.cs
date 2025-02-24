using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
public class AttachToSpline : Interactable
{
    public AntScript currentAnt;
    [SerializeField] private GameObject attachForSpline;
    private SplineAnimate animatedSpline;
    private AnimationHandler animHandler;
    [SerializeField] private string animName;
    [SerializeField] Quaternion rotation;
    [SerializeField] Vector3 detachSpeed;
    [SerializeField] Image promptImage;
    public float imageDistance;

    private void Awake()
    {
        animHandler = GetComponent<AnimationHandler>();
        animatedSpline = GetComponentInChildren<SplineAnimate>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<AntScript>() != null)
        {
            imageDistance = Vector2.Distance(promptImage.transform.position, currentAnt.transform.position);
            currentAnt = other.GetComponent<AntScript>();
            if(imageDistance >= 1.5)
            {
                promptImage.transform.position = Vector2.MoveTowards(promptImage.transform.position, currentAnt.transform.position, 1 * Time.deltaTime);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.GetComponent<AntScript>() == currentAnt) && (currentAnt.transform.parent == attachForSpline))
        {
            currentAnt = null;
        }

    }

    public void AttachObject()
    {
        if(currentAnt != null)
        {
            Rigidbody rb = currentAnt.GetComponent<Rigidbody>();
            currentAnt.gameObject.transform.SetParent(attachForSpline.transform);
            currentAnt.transform.rotation = rotation;
            rb.useGravity = false;
            animatedSpline.Play();
            animHandler.ToggleTrigger(animName);
        }
    }

    public void DetachObject()
    {
        Rigidbody rb = currentAnt.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(detachSpeed, ForceMode.Impulse);
        currentAnt.transform.SetParent(null);
        currentAnt.transform.rotation = rotation;
        currentAnt = null;
        animatedSpline.Restart(false);
      
    }

	public override void Interaction() {
        AttachObject();
        Debug.Log("Interacted");
	}
}
