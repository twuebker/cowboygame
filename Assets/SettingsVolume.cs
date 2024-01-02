using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsVolume : MonoBehaviour
{
    public AudioMixer audioMixer;
  public void SetVolume (float volume)
  {
    audioMixer.SetFloat("volume", volume);
  }
}
