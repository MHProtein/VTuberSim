using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;

namespace VTuber.ScheduleSystem.Schedule
{
    public class VDayScheduleSaveData
    {
        public uint currentEventIndex;
        public int currentTimeOfDay;
        public int dayIndex;
        public int eventIndex;
        public List<VScheduleEventSaveData> events;
    }

    public class VDaySchedule
    {
        private readonly int _dayIndex;
        private readonly VWeeklySchedule _weeklySchedule;
        private uint _currentEventIndex;
        private List<VScheduleEvent> _events = new();
        private TimeOfDay currentTimeOfDay;
        private int eventIndex;

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
            var startInt = (int)start;

            for (var i = 0; i < duration; i++)
            {
                var t = startInt + i;
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
            if (eventIndex >= _events.Count) _weeklySchedule.NextDay();
        }

        public void Reset(bool resetIndices)
        {
            foreach (var e in _events) e.Reset();
            _events.Clear();
            if (resetIndices)
            {
                currentTimeOfDay = TimeOfDay.Morning;
                eventIndex = 0;
            }
        }

        public VDayScheduleSaveData Save(VScript script)
        {
            return new VDayScheduleSaveData
            {
                events = _events.Select(e => e.Save(script)).ToList(),
                dayIndex = _dayIndex,
                eventIndex = eventIndex,
                currentTimeOfDay = (int)currentTimeOfDay,
                currentEventIndex = _currentEventIndex
            };
        }

        public static VDaySchedule Load(VDayScheduleSaveData saveData, VWeeklySchedule weeklySchedule, VScript script)
        {
            VDaySchedule daySchedule = new(weeklySchedule, saveData.dayIndex);
            daySchedule.eventIndex = saveData.eventIndex;
            daySchedule.currentTimeOfDay = (TimeOfDay)saveData.currentTimeOfDay;
            daySchedule._currentEventIndex = saveData.currentEventIndex;
            daySchedule._events = saveData.events.Select(eventSaveData => VScheduleEvent.Load(eventSaveData, script))
                .ToList();
            foreach (var scheduleEvent in daySchedule._events)
                scheduleEvent.SetDaySchedule(daySchedule, scheduleEvent.Coordinate);
            return daySchedule;
        }

        public VScheduleEvent GetCurrentEvent()
        {
            if (_events is null || _events.Count == 0)
                return null;
            return _events[(int)_currentEventIndex];
        }
    }
}