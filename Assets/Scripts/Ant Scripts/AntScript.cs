using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AntScript : Ant {


	[Header("Game Info")]
	
	private TurnManager turnManager;

	private void Start() {
		turnManager = FindFirstObjectByType<TurnManager>();
	}


}
