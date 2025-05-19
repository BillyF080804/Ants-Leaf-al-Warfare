using UnityEngine;

public class AnimationHandler : MonoBehaviour {
    public Animator animator;

    public void ToggleTrigger(string triggername) {
        animator.SetTrigger(triggername);
    }
}
