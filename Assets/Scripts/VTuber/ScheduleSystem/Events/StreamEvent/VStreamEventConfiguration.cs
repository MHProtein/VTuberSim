using UnityEngine.Serialization;

namespace VTuber.ScheduleSystem.Events
{
    public class VStreamEventConfiguration : VScheduleEventConfiguration
    {
        public int initialTurnCount;

        public override VScheduleEvent CreateEvent()
        {
            return new VStreamEvent(this);
        }
    }
}