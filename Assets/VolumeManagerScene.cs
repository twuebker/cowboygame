using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManagerScene : MonoBehaviour
{
    private static readonly string BackgroundPref = "BackgroundPref";
    private static readonly string SoundEffectsPref = "SoundEffectsPref";
    private float backgroundFloat, soundEffectsFloat;
    public AudioSource backgroundAudio;
    public AudioSource soundEffectsAudio;
    void Awake()
    {
        ContinueSettings();
    }

    private void ContinueSettings()
    {
        backgroundFloat = PlayerPrefs.GetFloat(BackgroundPref);
        soundEffectsFloat = PlayerPrefs.GetFloat(SoundEffectsPref);

        backgroundAudio.volume = backgroundFloat;
        soundEffectsAudio.volume = soundEffectsFloat;
        AudioManager.Instance.maxBgVolume = backgroundFloat;
    }
}
