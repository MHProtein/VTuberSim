using System;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.Schedule
{
    public class VDayScheduleSaveData
    {
        public List<VScheduleEventSaveData> events;
        public int dayIndex;
        public int eventIndex;
        public TimeOfDay currentTimeOfDay;
        public uint currentEventIndex;
    }
    
    public class VDaySchedule
    {
        private List<VScheduleEvent> _events = new();
        private VWeeklySchedule _weeklySchedule;
        private TimeOfDay currentTimeOfDay;
        private int _dayIndex = 0;
        private int eventIndex = 0;
        private uint _currentEventIndex = 0;

        public VDaySchedule(VWeeklySchedule weeklySchedule, int index)
        {
            currentTimeOfDay = TimeOfDay.Morning;
            _weeklySchedule = weeklySchedule;
            _dayIndex = index;
        }

        public void SetEvent(TimeOfDay timeOfDay, VScheduleEvent evt)
        {
            // Avoid adding duplicates
            if (_events.Any(e => (TimeOfDay)e.Coordinate.y == timeOfDay))
                return;

            if (evt.DaySchedule is null)
            {
                evt.SetDaySchedule(this, new Vector2Int(_dayIndex, (int)timeOfDay));
                _events.Add(evt);
            }
        }

        public VScheduleEvent GetEvent(TimeOfDay timeOfDay)
        {
            return _events.FirstOrDefault(e => (TimeOfDay)e.Coordinate.y == timeOfDay);
        }

        public List<VScheduleEvent> GetAllEvents()
        {
            return _events;
        }

        private List<TimeOfDay> GetTimeSlots(TimeOfDay start, int duration)
        {
            List<TimeOfDay> slots = new();
            int startInt = (int)start;

            for (int i = 0; i < duration; i++)
            {
                int t = startInt + i;
                if (t > (int)TimeOfDay.Evening)
                    break; 
                slots.Add((TimeOfDay)t);
            }

            return slots;
        }

        public VScheduleEvent GetNextEvent()
        {
            if (eventIndex >= _events.Count) return null;
            _currentEventIndex = (uint)eventIndex;
            return _events[eventIndex];
        }

        public void OnEventExecuted(VScheduleEvent e)
        {
            eventIndex++;
            VDebug.Log("Day: " + _dayIndex + " eventIndex: " + eventIndex);
            if(eventIndex >= _events.Count)
            { 
                _weeklySchedule.NextDay();
            }
        }

        public void Reset(bool resetIndices)
        {
            foreach (var e in _events)
            {
                e.Reset();
            }
            _events.Clear();
            if (resetIndices)
            {
                currentTimeOfDay = TimeOfDay.Morning;
                eventIndex = 0;
            }
        }

        public VDayScheduleSaveData Save()
        {
            return new VDayScheduleSaveData
            {
                events = _events.Select(e => e.Save()).ToList(),
                dayIndex = _dayIndex,
                eventIndex = eventIndex,
                currentTimeOfDay = currentTimeOfDay
            };
        }

        public static VDaySchedule Load(VDayScheduleSaveData saveData, VWeeklySchedule weeklySchedule)
        {
            VDaySchedule daySchedule = new(weeklySchedule, saveData.dayIndex);
            daySchedule.eventIndex = saveData.eventIndex;
            daySchedule.currentTimeOfDay = saveData.currentTimeOfDay;
            daySchedule._events = saveData.events.Select(VScheduleEvent.Load).ToList();
            return daySchedule;
        }

        public VScheduleEvent GetCurrentEvent()
        {
            return _events[(int)_currentEventIndex];
        }
    }
}
