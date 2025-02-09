using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeScript : MonoBehaviour
{
	public Ant.PlayerList ownedPlayer;

	public void InitialiseValues(Ant antInfoScript) {
		ownedPlayer = antInfoScript.ownedPlayer;
	}

	public void Attack() {
		
	}
}
