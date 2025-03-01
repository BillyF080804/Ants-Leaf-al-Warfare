using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BarbecueScript : MonoBehaviour
{
    private bool isFallen = false;
    private bool onFire = false;
    private DangerAdder adder;
    [Header("Interaction Variables")]
    [SerializeField] float igniteChance;
    [SerializeField] float damage;
    [SerializeField] GameObject fireArea;
    [SerializeField] GameObject igniteFx;
    [SerializeField] GameObject extinguishFx;
    [SerializeField] Transform dangerSpot;
    [SerializeField] Vector3 knockBackMin;
    [SerializeField] Vector3 knockBackMax;

    private void Awake()
    {
        adder = GetComponentInChildren<DangerAdder>();
    }
    public AntScript Fall(AntScript ant)
    {
        // Moves the ant that is currently in danger off to safety when barbecue falls, used in a Foreach in a different function.
            Rigidbody currentAntRB = ant.GetComponent<Rigidbody>();
            ant.transform.position = dangerSpot.position;
            currentAntRB.velocity = Vector3.zero;
            currentAntRB.velocity = new Vector3(Random.Range(knockBackMin.x, knockBackMax.x), Random.Range(knockBackMin.y, knockBackMax.y), Random.Range(knockBackMin.z, knockBackMax.z));

        //Method to publically set whether the Barbecue has fallen or not, used when the animation is triggered to knock Barbecue down
        isFallen = true;
        return ant;
    }

    public void HitTheBBQ()
    {
        adder.MoveAnts();
    }

    public void CheckForFire()
    {
        // performs a random calculation whether or not the Barbecue will set on fire this turn
        if(isFallen)
        {
            if (!onFire)
            {
                float lightUp = Random.Range(0f, 1f);
                if (lightUp < igniteChance)
                {
                    Debug.Log(lightUp);
                    SetFire();
                }
            }
            else
            {
                Extinguish();
            }
        }
    }

    public void SetFire()
    {
        onFire = true;
        fireArea.SetActive(true);
        if (igniteFx != null)
        {
            GameObject ignition = Instantiate(igniteFx);
            Destroy(ignition, 2f * Time.deltaTime);
        }
    }

    private void Extinguish()
    {
        onFire = false;
        fireArea.SetActive(false);
        if(extinguishFx != null)
        {
            GameObject extinguish = Instantiate(extinguishFx);
            Destroy(extinguish, 2f * Time.deltaTime);
        }
    }
}
