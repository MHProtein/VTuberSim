using VTuber.Character;
using VTuber.ScheduleSystem.Core;

namespace VTuber.ScheduleSystem.Events
{
    public class VStreamEvent : VScheduleEvent
    {
        public int InitialTurnCount { get; private set; }
        
        public VStreamEvent(VStreamEventConfiguration config) : base(config)
        {
            InitialTurnCount = config.initialTurnCount;
        }

        public override bool Execute(VCharacter player)
        {
            return false;
        }
    }
}