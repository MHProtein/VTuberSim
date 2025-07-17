using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Event;
using VTuber.ScheduleSystem.Schedule;

public class ScheduleUIManager : MonoBehaviour
{
    public GameObject eventButtonPrefab;
    public Transform eventButtonContainer;

    public GameObject timeSlotPrefab;
    public Transform timeSlotGrid; // 7x3 GridParent

    public List<ScheduleEventConfiguration> availableEvents;

    private WeeklySchedule weeklySchedule;
    private ScheduleEvent selectedEvent;

    private void Start()
    {
        weeklySchedule = new WeeklySchedule();

        PopulateEventButtons();
        CreateTimeSlots();
    }

    void PopulateEventButtons()
    {
        foreach (var config in availableEvents)
        {
            var obj = Instantiate(eventButtonPrefab, eventButtonContainer);
            var uiButton = obj.GetComponent<UIEventButton>();
            uiButton.Setup(config, OnEventSelected);
        }
    }

    void CreateTimeSlots()
    {
        for (int day = 0; day < 7; day++)
        {
            for (int t = 0; t < 3; t++)
            {
                var obj = Instantiate(timeSlotPrefab, timeSlotGrid);
                var timeUI = obj.GetComponent<TimeSlotUI>();
                timeUI.Setup(day, (TimeOfDay)t, OnTimeSlotClicked);
            }
        }
    }

    void OnEventSelected(ScheduleEventConfiguration config)
    {
        selectedEvent = new ScheduleEvent(config);
    }

    void OnTimeSlotClicked(int dayIndex, TimeOfDay time)
    {
        if (selectedEvent == null)
        {
            Debug.Log("<color=red>请先选择一个事件</color>");
            return;
        }

        int duration = selectedEvent.Duration; // 假设你在 ScheduleEvent 加了 Duration 属性
        if (weeklySchedule.CanScheduleEvent(dayIndex, time, duration))
        {
            for (int i = 0; i < duration; i++)
            {
                weeklySchedule.SetEvent(dayIndex, (TimeOfDay)((int)time + i), selectedEvent);
            }

            Debug.Log($"安排事件：{selectedEvent.EventName} 于 Day{dayIndex} {time} 起，持续{duration}段");
        }
        else
        {
            Debug.Log("<color=red>该时间段已被占用或超出范围</color>");
        }
    }
}
