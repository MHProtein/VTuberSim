using System.Collections.Generic;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Event;

namespace VTuber.ScheduleSystem.Schedule
{
    /// <summary>
    /// 一整周的安排，包含 7 天
    /// </summary>
    public class WeeklySchedule
    {
        private readonly List<DaySchedule> _days = new();

        public WeeklySchedule()
        {
            for (int i = 0; i < 7; i++)
                _days.Add(new DaySchedule());
        }

        public DaySchedule GetDay(int index)
        {
            if (index < 0 || index >= 7)
                throw new System.IndexOutOfRangeException("WeeklySchedule: index must be between 0 and 6.");
            return _days[index];
        }

        public List<DaySchedule> GetAllDays()
        {
            return _days;
        }

        public void SetEvent(int dayIndex, TimeOfDay timeOfDay, ScheduleEvent evt)
        {
            GetDay(dayIndex).SetEvent(timeOfDay, evt);
        }

        public ScheduleEvent GetEvent(int dayIndex, TimeOfDay timeOfDay)
        {
            return GetDay(dayIndex).GetEvent(timeOfDay);
        }
    }
}