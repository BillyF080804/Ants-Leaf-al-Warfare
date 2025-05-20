using UnityEngine;

public class QueenAttack : MonoBehaviour {
    [SerializeField] protected AudioPlayer audioPlayer;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip audioClip;

	private void Start() {
        audioPlayer = GetComponent<AudioPlayer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
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
