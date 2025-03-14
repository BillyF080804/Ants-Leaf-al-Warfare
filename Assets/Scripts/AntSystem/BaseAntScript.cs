using UnityEngine;

public class BaseAntScript : Ant {
    [Header("Base Ant Settings")]
    [SerializeField] private MeshRenderer hat;
    [SerializeField] private MeshRenderer leafWrap;

    public void ChangeAntColors(Color newColor) {
        hat.material.SetColor("_MainColours", newColor);
        leafWrap.material.SetColor("_MainColours", newColor);
    }
}
