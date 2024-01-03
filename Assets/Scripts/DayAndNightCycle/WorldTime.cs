using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WorldTime : MonoBehaviour
{

    public event EventHandler<TimeSpan> WorldTimeChanged;
    
    [SerializeField]
    private float dayLength;
    [SerializeField] 
    private string startTime; 
    private TimeSpan currentTime;
    private float minuteLength => dayLength / WorldTimeConstants.MinutesInDay;
    
    private void Start() 
    {
        currentTime = TimeSpan.Parse(startTime);
        StartCoroutine(AddMinute());
    }
    private IEnumerator AddMinute()
    {
        currentTime += TimeSpan.FromMinutes(1);
        WorldTimeChanged?.Invoke(this, currentTime);
        yield return new WaitForSeconds(minuteLength);
        StartCoroutine(AddMinute());
    }
}
