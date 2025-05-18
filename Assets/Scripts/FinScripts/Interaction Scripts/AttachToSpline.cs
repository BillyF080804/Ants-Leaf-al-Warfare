using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
public class AttachToSpline : Interactable
{
    [Header("Variables for launch")]
    [SerializeField] private GameObject attachForSpline;
    [SerializeField] Quaternion rotation;
    [SerializeField] Vector3 detachSpeed;
    public Ant currentAnt;

    [Header("Animation Variables")]
    [SerializeField] AnimationHandler uiHandler;
    [SerializeField] private string animName;
    [SerializeField] string enterTriggerName;
    [SerializeField] string exitTriggerName;
    [SerializeField] Image promptImage;
    public float imageDistance;

    private SplineAnimate animatedSpline;
    private AnimationHandler animHandler;
    private bool isLaunching = false;


    private void Awake()
    {
        animHandler = GetComponent<AnimationHandler>();
        animatedSpline = GetComponentInChildren<SplineAnimate>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ant>() != null)
        {
            uiHandler.ToggleTrigger(enterTriggerName);
            currentAnt = other.GetComponent<Ant>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(!isLaunching && imageDistance >= 1.5 && currentAnt.transform.position != null)
        {
            imageDistance = Vector2.Distance(promptImage.transform.position, currentAnt.transform.position);
            promptImage.transform.position = Vector2.MoveTowards(promptImage.transform.position, currentAnt.transform.position, 1 * Time.deltaTime);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.GetComponent<Ant>() == currentAnt) && (currentAnt.transform.parent == null))
        {
            uiHandler.ToggleTrigger(exitTriggerName);
            currentAnt = null;
        }

    }

    public void AttachObject()
    {
        if (currentAnt != null && !isLaunching)
        {
            currentAnt.SetCanMove(false);
            isLaunching = true;
            uiHandler.ToggleTrigger(exitTriggerName);
            Rigidbody rb = currentAnt.GetComponent<Rigidbody>();
            currentAnt.gameObject.transform.SetParent(attachForSpline.transform);
            currentAnt.gameObject.transform.localPosition = Vector3.zero;
            currentAnt.transform.rotation = rotation;
            rb.useGravity = false;
            animatedSpline.Play();
            animHandler.ToggleTrigger(animName);
        }
    }

    public void DetachObject()
    {
        currentAnt.SetCanMove(true);
        isLaunching = false;
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
