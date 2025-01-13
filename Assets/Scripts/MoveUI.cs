using System.Collections;
using UnityEngine;

public class MoveUI : MonoBehaviour {
    //Public function
    public void StartMoveUI(LerpType lerpType, GameObject objectToLerp, Vector2 startPosition, Vector2 endPosition, float duration) {
        StartCoroutine(MoveUICoroutine(lerpType, objectToLerp, startPosition, endPosition, duration));
    }

    //Coroutine to move UI
    private IEnumerator MoveUICoroutine(LerpType lerpType, GameObject objectToLerp, Vector2 startPosition, Vector2 endPosition, float duration) {
        //Create time variable
        float time = 0;

        //Lerp to actually move the UI
        while (time < duration) {
            objectToLerp.GetComponent<RectTransform>().anchoredPosition = LerpLibrary.UILerp(startPosition, endPosition, lerpType, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        objectToLerp.GetComponent<RectTransform>().anchoredPosition = endPosition;
    }
}
