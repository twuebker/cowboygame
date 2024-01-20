using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    private AudioSource soundEffects;

    private AudioSource backgroundMusic;
    public float maxBgVolume;

    private static AudioManager instance;
    public static AudioManager Instance {
        get {
            if (instance == null) {
                Debug.Log("AudioManager instance is null!");
            }
            return instance;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {   
        soundEffects = GameObject.Find("SoundEffects").GetComponent<AudioSource>();
        backgroundMusic = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
        backgroundMusic.clip = clips[1];
        backgroundMusic.Play();
        if(soundEffects == null || backgroundMusic == null) {
            Debug.Log("Failed to load soundEffect or bgMusic source");
        }
        instance = this;
    }

    public void PlayGunshot() {
        if(clips.Length == 0) {
            Debug.Log("No AudioClips attached!");
            return;
        }
        soundEffects.clip = clips[0];
        soundEffects.Play();
    }

    public void PlayDayClip() {
        StartCoroutine(FadeTrack(clips[1]));
    }

    public void PlayNightClip() {
        StartCoroutine(FadeTrack(clips[2]));
    }

    private IEnumerator FadeTrack(AudioClip clip) {
        float timeElapsed = 0;
        float timeToFade = 0.5f;
        while(timeElapsed < timeToFade) {
            backgroundMusic.volume = Mathf.Lerp(maxBgVolume, 0, timeElapsed/timeToFade);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        timeElapsed = 0;
        timeToFade = 0.5f;
        backgroundMusic.clip = clip;
        backgroundMusic.Play();
        while(timeElapsed < timeToFade) {
            backgroundMusic.volume = Mathf.Lerp(0, maxBgVolume, timeElapsed/timeToFade);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

}
