using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : MonoBehaviour {
    private string dropType = string.Empty;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log(dropType);
        }
    }

    public void SetDropType(string _dropType) {
        dropType = _dropType;
    }
}