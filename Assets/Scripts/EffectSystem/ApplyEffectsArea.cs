using UnityEngine;

public class ApplyEffectsArea : MonoBehaviour
{
	[SerializeField]
	EffectScript effectScript;

	private void OnTriggerEnter(Collider other) {
		if(other.GetComponent<Ant>() != null) {
			effectScript.AddEffect(other.GetComponent<Ant>());
			effectScript.ApplyEffect(other.GetComponent<Ant>());
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.GetComponent<Ant>() != null) {
			effectScript.RemoveEffect(other.GetComponent<Ant>());
		}
	}
}
