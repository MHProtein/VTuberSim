
using System;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using System.Collections.Generic;
using VTuber.Character;

namespace VTuber.ScheduleSystem.Schedule
{
    /// <summary>
    /// 表示一天中的三段时间安排
    /// </summary>
    public class DaySchedule
    {
        private readonly Dictionary<TimeOfDay, VScheduleEvent> _events = new();
        
        private VCharacter _character;
        private VWeeklySchedule _vWeeklySchedule;
        
        private class ScheduledSlot
        {
            public VScheduleEvent Event;
            public bool IsPrimarySlot; // 只在第一个时间段执行
        }

        private readonly Dictionary<TimeOfDay, ScheduledSlot> _slots = new();
        
        private TimeOfDay currentTimeOfDay;
        
        public bool CanScheduleEvent(TimeOfDay startTime, int duration)
        {
            var times = GetTimeSlots(startTime, duration);
            foreach (var t in times)
            {
                if (_events.ContainsKey(t)) return false;
            }
            return true;
        }

        public DaySchedule(VWeeklySchedule vWeeklySchedule, VCharacter character)
        {
            currentTimeOfDay = TimeOfDay.Morning;
            _character = character;
            _vWeeklySchedule = vWeeklySchedule;
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
            evt.SetDaySchedule(this);
            _slots[timeOfDay] = new ScheduledSlot { Event = evt, IsPrimarySlot = isPrimary };
            _events[timeOfDay] = evt;
        }

        public VScheduleEvent GetEvent(TimeOfDay timeOfDay)
        {
            if (_slots.TryGetValue(timeOfDay, out var slot))
                return slot?.Event;
            return null;
        }

        public bool IsPrimary(TimeOfDay timeOfDay)
        {
            if (_slots.TryGetValue(timeOfDay, out var slot))
                return slot?.IsPrimarySlot ?? false;
            return false;
        }
        
        public Dictionary<TimeOfDay, VScheduleEvent> GetAllEvents()
        {
            return new Dictionary<TimeOfDay, VScheduleEvent>(_events);
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

        public void Execute()
        {
            var e = _slots[currentTimeOfDay].Event;
            e.Execute(_character);
            for(int i = 0; i < e.Duration; i++)
            {
                currentTimeOfDay = NextTimeOfDay();
            }
        }

        public void NextEvent()
        {
            if (currentTimeOfDay == TimeOfDay.End)
            {
                _vWeeklySchedule.NextDay();
                return;
            }

            Execute();
        }
        
    }
}