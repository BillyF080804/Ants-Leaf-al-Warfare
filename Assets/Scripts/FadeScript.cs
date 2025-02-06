using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeScript : MonoBehaviour {
    public void FadeInUI(float fadeDuration, GameObject fadeObject) {
        fadeObject.SetActive(true);
        StartCoroutine(FadeObject(true, fadeDuration, fadeObject));
    }

    public void FadeInUI(float fadeDuration) {
        StartCoroutine(FadeObject(true, fadeDuration, gameObject));
    }

    public void FadeOutUI(float fadeDuration, GameObject fadeObject) {
        StartCoroutine(FadeObject(false, fadeDuration, fadeObject));
    }

    public void FadeOutUI(float fadeDuration) {
        StartCoroutine(FadeObject(false, fadeDuration, gameObject));
    }

    public void FadeInAndOutUI(float fadeDuration, float timeBetweenFades, GameObject fadeObject) {
        fadeObject.SetActive(true);
        StartCoroutine(FadeObjectInAndOut(fadeDuration, timeBetweenFades, fadeObject));
    }

    public void FadeInAndOutUI(float fadeDuration, float timeBetweenFades) {
        StartCoroutine(FadeObjectInAndOut(fadeDuration, timeBetweenFades, gameObject));
    }

    //Fades an object in waits for x amount of time and then fades object out
    private IEnumerator FadeObjectInAndOut(float fadeDuration, float timeBetweenFades, GameObject fadeObject) {
        StartCoroutine(FadeObject(true, fadeDuration, fadeObject));
        yield return new WaitForSeconds(timeBetweenFades);
        StartCoroutine(FadeObject(false, fadeDuration, fadeObject));
    }

    //Fades an object in or out over a set duration
    private IEnumerator FadeObject(bool fadeIn, float fadeDuration, GameObject fadeObject) {
        float timeElapsed = 0.0f;
        CanvasGroup objCanvasGroup = fadeObject.GetComponent<CanvasGroup>();

        while (timeElapsed < fadeDuration) {
            if (fadeIn == true) {
                objCanvasGroup.alpha = Mathf.Lerp(0, 1, timeElapsed / fadeDuration);
            }
            else {
                objCanvasGroup.alpha = Mathf.Lerp(1, 0, timeElapsed / fadeDuration);
            }
            
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (fadeIn == false) {
            objCanvasGroup.alpha = 0;
            fadeObject.SetActive(false);
        }
        else {
            objCanvasGroup.alpha = 1;
        }
    }
}