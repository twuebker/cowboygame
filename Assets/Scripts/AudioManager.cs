using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    private AudioSource soundEffects;

    private AudioSource backgroundMusic;

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

}
