using System.Collections;
using UnityEngine;

public class MoveUI : MonoBehaviour {
    //Public function
    public void StartMoveUI(LerpType lerpType, GameObject objectToLerp, Vector2 startPosition, Vector2 endPosition, float duration) {
        StartCoroutine(MoveUICoroutine(lerpType, objectToLerp, startPosition, endPosition, duration, false));
    }

    public void StartMoveUI(LerpType lerpType, GameObject objectToLerp, Vector2 startPosition, Vector2 endPosition, float duration, bool disableWhenFinished) {
        StartCoroutine(MoveUICoroutine(lerpType, objectToLerp, startPosition, endPosition, duration, disableWhenFinished));
    }

    public void StartMoveUI(LerpType lerpType, Vector2 startPosition, Vector2 endPosition, float duration) {
        StartCoroutine(MoveUICoroutine(lerpType, gameObject, startPosition, endPosition, duration, false));
    }

    public void StartMoveUI(LerpType lerpType, Vector2 startPosition, Vector2 endPosition, float duration, bool disableWhenFinished) {
        StartCoroutine(MoveUICoroutine(lerpType, gameObject, startPosition, endPosition, duration, disableWhenFinished));
    }

    //Coroutine to move UI
    private IEnumerator MoveUICoroutine(LerpType lerpType, GameObject objectToLerp, Vector2 startPosition, Vector2 endPosition, float duration, bool disableWhenFinished) {
        //Create time variable
        float time = 0;

        //Lerp to actually move the UI
        while (time < duration) {
            objectToLerp.GetComponent<RectTransform>().anchoredPosition = LerpLibrary.UILerp(startPosition, endPosition, lerpType, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        objectToLerp.GetComponent<RectTransform>().anchoredPosition = endPosition;

        if (disableWhenFinished) {
            objectToLerp.SetActive(false);
        }
    }
}
