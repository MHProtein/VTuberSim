
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

        public void SetEvent(TimeOfDay timeOfDay, ScheduleEvent evt)
        {
            _events[timeOfDay] = evt;
        }

        public ScheduleEvent GetEvent(TimeOfDay timeOfDay)
        {
            _events.TryGetValue(timeOfDay, out var evt);
            return evt;
        }

        public Dictionary<TimeOfDay, ScheduleEvent> GetAllEvents()
        {
            return new Dictionary<TimeOfDay, ScheduleEvent>(_events);
        }
    }
}