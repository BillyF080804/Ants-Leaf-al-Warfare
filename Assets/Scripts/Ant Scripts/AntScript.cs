using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AntScript : Ant {

	[Header("Ant Info")]
	public AntSO antInfo;

	[Header("Game Info")]
	public bool hasHadTurn = false;
	private TurnManager turnManager;

	private void Start() {
		turnManager = FindFirstObjectByType<TurnManager>();
	}



	public void OnJump() {
        if (canJump) {
            Vector2 Force = new Vector2(0, antInfo.jumpHeight);
            GetComponent<Rigidbody>().AddForce(Force, ForceMode.Impulse);
            canJump = false;
        }
    }

	public void TakeDamage(int Damage) {
		antInfo.health -= Damage;
	}

	private void Update() {
		transform.Translate(antInfo.moveSpeed * Time.deltaTime * moveVector);
	}


}
