using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;
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
            if (!CanExecute(player))
                return false;
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStreamEventStart, new Dictionary<string, object>()
            {
                {"Event", this}
            });
            return true;
        }
    }
}