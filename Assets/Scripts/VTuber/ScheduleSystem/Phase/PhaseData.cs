using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Event;

namespace VTuber.ScheduleSystem.Phase
{
    /// <summary>
    /// 某个时间段（如早上）的事件快照
    /// </summary>
    public class PhaseData
    {
        public int DayIndex { get; private set; }
        public TimeOfDay TimeOfDay { get; private set; }
        public VScheduleEvent Event { get; private set; }

        public PhaseData(int dayIndex, TimeOfDay timeOfDay, VScheduleEvent vScheduleEvent)
        {
            DayIndex = dayIndex;
            TimeOfDay = timeOfDay;
            Event = vScheduleEvent;
        }

        public override string ToString()
        {
            return $"Day {DayIndex}, {TimeOfDay}: {Event?.EventName ?? "None"}";
        }
    }
}