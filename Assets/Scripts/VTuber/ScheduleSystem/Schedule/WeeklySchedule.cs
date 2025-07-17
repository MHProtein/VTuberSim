using System.Collections.Generic;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Event;
using VTuber.Core.Foundation;

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
        
        private List<TimeOfDay> GetTimeRange(TimeOfDay start, int duration)
        {
            var times = new List<TimeOfDay>();
            var values = (TimeOfDay[])System.Enum.GetValues(typeof(TimeOfDay));
            int startIndex = (int)start;

            for (int i = 0; i < duration && startIndex + i < values.Length; i++)
            {
                times.Add(values[startIndex + i]);
            }

            return times;
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

        public void SetEvent(int dayIndex, TimeOfDay startTime, ScheduleEvent evt)
        {
            var duration = evt.Duration;
            var day = GetDay(dayIndex);
            var times = GetTimeRange(startTime, duration);

            foreach (var time in times)
            {
                day.SetEvent(time, evt, time == startTime);
            }
        }
        
        public bool CanScheduleEvent(int dayIndex, TimeOfDay startTime, int duration)
        {
            var timeValues = (TimeOfDay[])System.Enum.GetValues(typeof(TimeOfDay));

            // 超出当天时间段数量，直接非法
            if ((int)startTime + duration > timeValues.Length)
            {
                VDebug.Log($"<color=red>无法安排：day{dayIndex}，startTime {startTime}，duration 超出时间范围</color>");
                return false;
            }

            var day = GetDay(dayIndex);
            foreach (var time in GetTimeRange(startTime, duration))
            {
                if (day.GetEvent(time) != null)
                {
                    VDebug.Log($"<color=red>无法安排：day{dayIndex}，startTime {startTime}，时间段 {time} 已被占用</color>");
                    return false;
                }
            }

            return true;
        }

        public ScheduleEvent GetEvent(int dayIndex, TimeOfDay timeOfDay)
        {
            return GetDay(dayIndex).GetEvent(timeOfDay);
        }
    }
}