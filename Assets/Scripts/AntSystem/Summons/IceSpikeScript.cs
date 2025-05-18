using UnityEngine;

public class IceSpikeScript : MonoBehaviour
{
	[SerializeField] int damageToDeal;
	[SerializeField] float timeTilDeath;
	[SerializeField] EffectScript effectScript;


	private void Start() {
		Destroy(gameObject, timeTilDeath);
	}


	public void InitialiseValues(int _damageToDeal, EffectScript _effectScript) {
		damageToDeal = _damageToDeal;
		effectScript = _effectScript;
	}


	private void OnTriggerEnter(Collider other) {
		if (other.TryGetComponent<Ant>(out Ant antScript)) {
			antScript.TakeDamage(damageToDeal);
			effectScript.AddEffect(antScript);
			Destroy(gameObject);
		}
	}

	
}
