
using System;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.ScheduleSystem.Schedule
{
    /// <summary>
    /// 表示一天中的三段时间安排
    /// </summary>
    public class DaySchedule
    {
        private readonly List<VScheduleEvent> _events = new();
        
        private VCharacter _character;
        private VWeeklySchedule _vWeeklySchedule;
        
        private class ScheduledSlot
        {
            public VScheduleEvent Event;
            public bool IsPrimarySlot; // 只在第一个时间段执行
        }

        private readonly Dictionary<TimeOfDay, ScheduledSlot> _slots = new();
        
        private TimeOfDay currentTimeOfDay;
        private int _dayIndex = 0;
        private int eventIndex = 0;

        public DaySchedule(VWeeklySchedule vWeeklySchedule, VCharacter character, int index)
        {
            currentTimeOfDay = TimeOfDay.Morning;
            _character = character;
            _vWeeklySchedule = vWeeklySchedule;
            _dayIndex = index;
        }
        
        public TimeOfDay NextTimeOfDay()
        {
            switch (currentTimeOfDay)
            {
                case TimeOfDay.Morning:
                    return TimeOfDay.Afternoon;
                case TimeOfDay.Afternoon:
                    return TimeOfDay.Evening;
                case TimeOfDay.Evening:
                    return TimeOfDay.End;
            }

            return TimeOfDay.End;
        }
        
        public void SetEvent(TimeOfDay timeOfDay, VScheduleEvent evt, bool isPrimary)
        {
            if (evt.DaySchedule is null)
            {
                evt.SetDaySchedule(this, new Vector2Int(_dayIndex, (int)timeOfDay));
                _events.Add(evt);
            }
        }

        public VScheduleEvent GetEvent(TimeOfDay timeOfDay)
        {
            if (_slots.TryGetValue(timeOfDay, out var slot))
                return slot?.Event;
            return null;
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
                    break; // 超出一天范围
                slots.Add((TimeOfDay)t);
            }

            return slots;
        }

        public VScheduleEvent GetNextEvent()
        {
            var e = _events[eventIndex];

            return e;
        }

        public void OnEventExecuted(VScheduleEvent e)
        {
            eventIndex++;
        }

        public VScheduleEvent NextEvent()
        {
            if (eventIndex > _events.Count - 1)
            {
                return _vWeeklySchedule.NextDay();
            }

            return GetNextEvent();
        }

        public void Reset(bool resetIndices)
        {
            foreach (var e in _events)
            {
                e.Reset();
            }
            _events.Clear();
            _slots.Clear();
            if (resetIndices)
            {
                currentTimeOfDay = TimeOfDay.Morning;
                eventIndex = 0;
            }
        }
    }
}