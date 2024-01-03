using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.Events;

public class WorldTimeWatcher : MonoBehaviour
{
    [SerializeField] 
    private WorldTime worldTime;
    [SerializeField] 
    private List<Schedule> schedule;

    private void Start() {
        worldTime.WorldTimeChanged += CheckSchedule;
    }

    private void OnDestroy() {
        worldTime.WorldTimeChanged -= CheckSchedule;
    }

    private void CheckSchedule(object sender, TimeSpan newTime) {
        var schedules = schedule.Where(s => s.Hour == newTime.Hours && s.Minute == newTime.Minutes);
        foreach(var s in schedules) {
            s?.action?.Invoke();
        }
    }

    [Serializable]
    private class Schedule {
        public int Hour;
        public int Minute;
        public UnityEvent action;

    }
}
