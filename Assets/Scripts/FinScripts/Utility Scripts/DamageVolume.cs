using UnityEngine;

public class DamageVolume : MonoBehaviour {
    [SerializeField] private int damageTaken;
    [SerializeField] private bool causesEffect;
    [SerializeField] private EffectScript effectCause;

    private void OnTriggerEnter(Collider other) {
        Ant ant = other.GetComponent<Ant>();
        if (ant.GetComponent<Ant>() != null) {
            ant.TakeDamage(damageTaken);
            Debug.Log("DamagedAnt");
            if (causesEffect) {
                ant.effects.Add(effectCause);
                Debug.Log(ant.effects.ToString());
            }
        }
    }


}
