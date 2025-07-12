
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Event;
using System.Collections.Generic;
namespace VTuber.ScheduleSystem.Schedule
{
    /// <summary>
    /// 表示一天中的三段时间安排
    /// </summary>
    public class DaySchedule
    {
        private readonly Dictionary<TimeOfDay, ScheduleEvent> _events = new();

        public bool CanScheduleEvent(TimeOfDay startTime, int duration)
        {
            var times = GetTimeSlots(startTime, duration);
            foreach (var t in times)
            {
                if (_events.ContainsKey(t)) return false;
            }
            return true;
        }
        
        private class ScheduledSlot
        {
            public ScheduleEvent Event;
            public bool IsPrimarySlot; // 只在第一个时间段执行
        }

        private readonly Dictionary<TimeOfDay, ScheduledSlot> _slots = new();

        public void SetEvent(TimeOfDay timeOfDay, ScheduleEvent evt, bool isPrimary)
        {
            _slots[timeOfDay] = new ScheduledSlot { Event = evt, IsPrimarySlot = isPrimary };
        }

        public ScheduleEvent GetEvent(TimeOfDay timeOfDay)
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
        
        public Dictionary<TimeOfDay, ScheduleEvent> GetAllEvents()
        {
            return new Dictionary<TimeOfDay, ScheduleEvent>(_events);
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
    }

}