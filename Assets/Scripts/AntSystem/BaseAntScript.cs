using UnityEngine;

public class BaseAntScript : Ant {
    [Header("Base Ant Settings")]
    [SerializeField] private SkinnedMeshRenderer hat;
    [SerializeField] private SkinnedMeshRenderer leafWrap;

    public void ChangeAntColors(Color newColor) {
        hat.material.SetColor("_MainColours", newColor);
        leafWrap.material.SetColor("_MainColours", newColor);
    }
}
