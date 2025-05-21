using UnityEngine;

public class FadeAudioScript : MonoBehaviour {
    [Header("Fade Settings")]
    private bool fadeOut = false;
    private bool fadeIn = false;
    private float fadeSpeed;
    private float maximumVolume;
    private AudioSource audioSource;

    //Sets values for the audio fade
    public void AudioFade(string fadeType, AudioSource newAudioSource, float speed, float maxVolume) {
        if (fadeType == "Open") {
            fadeOut = false;
            fadeIn = true;
            audioSource = newAudioSource;
            fadeSpeed = speed;
            maximumVolume = maxVolume;
        } 
        else if (fadeType == "Close") {
            fadeIn = false;
            fadeOut = true;
            audioSource = newAudioSource;
            fadeSpeed = speed;
            maximumVolume = maxVolume;
        }
    }

    private void Update() {
        //Fades the audio In
        if (fadeIn == true && fadeOut == false) {
            if (audioSource.volume <= maximumVolume) {
                audioSource.volume += Time.deltaTime * fadeSpeed;

                if (audioSource.volume >= maximumVolume) {
                    fadeIn = false;
                }
            }
        }

        //Fades the audio out
        if (fadeOut == true && fadeIn == false) {
            if (audioSource.volume >= 0) {
                audioSource.volume -= Time.deltaTime * fadeSpeed;

                if (audioSource.volume == 0) {
                    fadeOut = false;
                }
            }
        }
    }
}