using UnityEngine;

[CreateAssetMenu(menuName = "Ant System/New Ant Type", fileName = "New Ant")]
public class AntSO : ScriptableObject {
    [Header("Ant Cosmetics")]
    public GameObject antPrefab = null;
    public Color antColor = Color.black;

    [Header("Ant Attributes")]
    public string antName = "";
    public float moveSpeed = 4;
    public int jumpHeight = 5;
    public int health = 100;


    public enum QueenType {
        Ice,
        Bee,
        Pharaoh,
        Weaver,
        Dracula,
        Bullet
    }


    [Header("Queen Attributes")]
    public bool IsQueen = false;
    public QueenType queenType;
    public int damage = 1;
    public QueenAttack queenAttack;
    public string queenArchetype;
    public string description;
    [SerializeField] AudioClip audioClip;
}
