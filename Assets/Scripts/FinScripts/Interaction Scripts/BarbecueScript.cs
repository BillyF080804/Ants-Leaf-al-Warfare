using UnityEngine;

public class BarbecueScript : MonoBehaviour {
    private bool isFallen = false;
    private bool onFire = false;
    private DangerAdder adder;
    [Header("Numbers")]
    [SerializeField] float igniteChance;
    [SerializeField] float damage;
    [SerializeField] Vector3 knockBackMin;
    [SerializeField] Vector3 knockBackMax;

    [Header("Game Objects")]
    [SerializeField] GameObject fireArea;
    [SerializeField] GameObject igniteFx;
    [SerializeField] GameObject extinguishFx;

    [Header("Transforms")]
    [SerializeField] Transform fxSpawnPoint;
    [SerializeField] Transform dangerSpot;


    private void Awake() {
        adder = GetComponentInChildren<DangerAdder>();
    }

    private void Start() {

    }
    public Ant Fall(Ant ant) {
        // Moves the ant that is currently in danger off to safety when barbecue falls, used in a Foreach in a different function.
        Rigidbody currentAntRB = ant.GetComponent<Rigidbody>();
        ant.transform.position = dangerSpot.position;
        currentAntRB.velocity = Vector3.zero;
        currentAntRB.velocity = new Vector3(Random.Range(knockBackMin.x, knockBackMax.x), Random.Range(knockBackMin.y, knockBackMax.y), Random.Range(knockBackMin.z, knockBackMax.z));

        //Method to publically set whether the Barbecue has fallen or not, used when the animation is triggered to knock Barbecue down
        isFallen = true;
        return ant;
    }

    public void HitTheBBQ() {
        adder.MoveAnts();
    }

    public void CheckForFire() {
        // performs a random calculation whether or not the Barbecue will set on fire this turn
        if (isFallen) {
            if (!onFire) {
                float lightUp = Random.Range(0f, 1f);
                if (lightUp < igniteChance) {
                    Debug.Log(lightUp);
                    SetFire();
                }
            }
            else {
                Extinguish();
            }
        }
    }

    public void SetFire() {
        onFire = true;
        fireArea.SetActive(true);
        if (igniteFx != null) {
            Debug.Log("On fire");
            GameObject ignition = Instantiate(igniteFx, fxSpawnPoint);
            Debug.Log(ignition.name);
            Destroy(ignition, 2f);
        }
    }

    private void Extinguish() {
        Debug.Log("Extinguished");
        onFire = false;
        fireArea.SetActive(false);
        if (extinguishFx != null) {
            GameObject extinguish = Instantiate(extinguishFx, fxSpawnPoint);
            Destroy(extinguish, 2f);
        }
    }
}
