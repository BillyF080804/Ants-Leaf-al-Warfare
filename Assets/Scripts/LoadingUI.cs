using UnityEngine;

public class LoadingUI : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private GameObject shutterLeft;
    [SerializeField] private GameObject shutterRight;
    [SerializeField] private GameObject leaf;

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public void OpenShutter() {
        shutterLeft.GetComponent<MoveUI>().StartMoveUI(LerpType.InOut, Vector2.zero, new Vector2(-1100, 0), 1.0f);
        shutterRight.GetComponent<MoveUI>().StartMoveUI(LerpType.InOut, Vector2.zero, new Vector2(1100, 0), 1.0f);
        leaf.GetComponent<MoveUI>().StartMoveUI(LerpType.InBack, Vector2.zero, new Vector2(0, -800), 1.0f);
    }

    public void CloseShutter() {
        shutterLeft.GetComponent<MoveUI>().StartMoveUI(LerpType.OutBounce, new Vector2(-1100, 0), Vector2.zero, 1.0f);
        shutterRight.GetComponent<MoveUI>().StartMoveUI(LerpType.OutBounce, new Vector2(1100, 0), Vector2.zero, 1.0f);
        leaf.GetComponent<MoveUI>().StartMoveUI(LerpType.OutBack, new Vector2(0, 800), Vector2.zero, 1.0f);
    }
}