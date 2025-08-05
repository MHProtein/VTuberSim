using System.Collections.Generic;
using VTuber.Core.EventCenter;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Events.DialogueEvent;

namespace VTuber.BattleSystem.Core.ScriptSystem
{
    public class VScript
    {
        private List<VPhase> Phases => _configuration.phases;
        private VScriptConfiguration _configuration;
        public VPhase CurrentPhase => _currentPhase;
        private VPhase _currentPhase;

        public VScript(VScriptConfiguration configuration)
        {
            _configuration = configuration;
        }

        public VScheduleEvent BeginScript()
        {
            _currentPhase = Phases[0];
            return _currentPhase.GetStartEvent();
        }
        
        public List<VSpecialEventData> GetSpecialEvents(int weekIndex)
        {
            List<VSpecialEventData> events = new List<VSpecialEventData>();
            foreach (var phase in Phases)
            {
                if(phase.IsInPhase(weekIndex))
                    events.AddRange(phase.GetSpecialEventData());
            }
            return events;
        }

        public void OnEventExecuted(VScheduleEvent e)
        {
            if (e.IsPhaseStart)
            {
                _currentPhase = e.Phase;
            }   
            else if (e.IsPhaseEndingEvent)
            {
                _currentPhase = null;
            }
        }
    }
}