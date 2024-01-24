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
    private AudioSource backgroundMusic2;
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
        backgroundMusic = GetChildByName("BackgroundMusic");
        backgroundMusic2 = GetChildByName("BackgroundMusic2");
        soundEffects = GetChildByName("SoundEffects");
        if(backgroundMusic2 != null) {
            backgroundMusic2.Stop();
        }
        if(soundEffects == null || backgroundMusic == null || backgroundMusic2 == null) {
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
       // Make sure both audio sources are not null
        if (backgroundMusic != null && backgroundMusic2 != null)
        {
            StartCoroutine(CrossFade(backgroundMusic2, backgroundMusic, 1.0f));
        }
    }

    public void PlayNightClip() {
       // Make sure both audio sources are not null
        if (backgroundMusic != null && backgroundMusic2 != null)
        {
            StartCoroutine(CrossFade(backgroundMusic, backgroundMusic2, 1.0f));
        }
    }

        // Helper method to find child by name
    private AudioSource GetChildByName(string childName)
    {
        Transform childTransform = transform.Find(childName);

        if (childTransform != null)
        {
            return childTransform.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogError(childName + " not found!");
            return null;
        }
    }

    // Coroutine for crossfading between two audio sources
    private IEnumerator CrossFade(AudioSource fadeOutSource, AudioSource fadeInSource, float duration)
    {
        float elapsedTime = 0f;
        float startVolume = fadeOutSource.volume;
        fadeInSource.Play();
        while (elapsedTime < duration)
        {
            fadeOutSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            fadeInSource.volume = Mathf.Lerp(0f, startVolume, elapsedTime / duration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure a clean transition and avoid potential volume issues
        fadeOutSource.volume = 0f;
        fadeInSource.volume = startVolume;

        // Pause and reset the faded out audio source
        fadeOutSource.Pause();
        fadeOutSource.time = 0;

        // Unpause and play the new audio source
        fadeInSource.UnPause();
    }

}
