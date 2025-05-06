using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenAttack : MonoBehaviour {
	[SerializeField] protected AudioPlayer audioPlayer;
	
	private void Start() {
		audioPlayer = GetComponent<AudioPlayer>();
	}

	public virtual void ActivateAttack(int attackLevel, Ant antInfoScript) {
		Debug.Log("Not Updated");
	}

	public virtual void ActivateAttack(int attackLevel, Ant antInfoScript, Vector3 position) {
		Debug.Log("Not Updated");
	}

	public virtual void ActivateAttack(int attackLevel, Ant antInfoScript, Vector3 position, TurnManager turnManager) {
		Debug.Log("Not Updated");
	}

	public virtual void InitialiseValues(GameObject attackInfo) {

	}
	
}
