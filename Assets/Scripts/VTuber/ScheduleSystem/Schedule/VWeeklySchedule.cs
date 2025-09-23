using System.Collections.Generic;
using System.Linq;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;

namespace VTuber.ScheduleSystem.Schedule
{
    public class VWeeklyScheduleSaveData
    {
        public List<VDayScheduleSaveData> days;
        public int currentDayIndex;
    }

    public class VWeeklySchedule
    {
        private List<VDaySchedule> _days = new();
        private int _currentDayIndex = 0;
        public VWeeklySchedule()
        {
            for (int i = 0; i < 7; i++)
                _days.Add(new VDaySchedule(this, i));
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
        
        public VDaySchedule GetDay(int index)
        {
            if (index < 0 || index >= 7)
                throw new System.IndexOutOfRangeException("WeeklySchedule: index must be between 0 and 6.");
            return _days[index];
        }

        public List<VDaySchedule> GetAllDays()
        {
            return _days;
        }

        public void SetEvent(int dayIndex, TimeOfDay startTime, VScheduleEvent evt)
        {
            var duration = evt.Duration;
            var day = GetDay(dayIndex);
            var times = GetTimeRange(startTime, duration);

            foreach (var time in times)
            {
                day.SetEvent(time, evt);
            }
        }

        public VScheduleEvent GetEvent(int dayIndex, TimeOfDay timeOfDay)
        {
            return GetDay(dayIndex).GetEvent(timeOfDay);
        }

        public VScheduleEvent GetCurrentEvent()
        {
            return GetDay(_currentDayIndex).GetCurrentEvent();
        }

        public VScheduleEvent BeginExecution()
        {
            _currentDayIndex = 0;
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnWeekStart, new Dictionary<string, object>());
            return _days[_currentDayIndex].GetNextEvent();
        }

        public void Reset(bool resetIndices)
        {
            foreach (var day in _days)
            {
                day.Reset(resetIndices);
            }
            if(resetIndices)
                _currentDayIndex = 0;
        }

        public void NextDay()
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnDayEnd,
                new Dictionary<string, object>()
                {
                    { "DayIndex", _currentDayIndex },
                });
            if (_currentDayIndex == 6)
            {
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnWeekEnd,
                    new Dictionary<string, object>()
                    {
                        { "DayIndex", _currentDayIndex },
                    });
            }
            _currentDayIndex++;
        }

        public VScheduleEvent NextEvent()
        {
            if (_currentDayIndex >= _days.Count)
                return null;
            return _days[_currentDayIndex].GetNextEvent();
        }

        public VWeeklyScheduleSaveData Save(VScript script)
        {
            return new VWeeklyScheduleSaveData
            {
                days = _days.Select(day => day.Save(script)).ToList(),
                currentDayIndex = _currentDayIndex,
            };
        }
        
        public static VWeeklySchedule Load(VWeeklyScheduleSaveData saveData, VScript script)
        {
            VWeeklySchedule weeklySchedule = new();
            weeklySchedule._days = saveData.days.Select(day => VDaySchedule.Load(day, weeklySchedule, script)).ToList();
            weeklySchedule._currentDayIndex = saveData.currentDayIndex;
            return weeklySchedule;
        }
    }
}