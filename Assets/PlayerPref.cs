using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPref : MonoBehaviour
{
    [SerializeField] private string key;
    
    public void SetFloat(float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }
}
