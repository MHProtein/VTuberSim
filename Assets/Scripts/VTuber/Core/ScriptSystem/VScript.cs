using System.Collections.Generic;
using VTuber.Core.EventCenter;
using VTuber.ScheduleSystem.Events;

namespace VTuber.BattleSystem.Core.ScriptSystem
{
    public class VScript
    {
        private List<VPhase> Phases => _configuration.phases;
        private VScriptConfiguration _configuration;

        public VScript(VScriptConfiguration configuration)
        {
            _configuration = configuration;
        }


        
        public List<VSpecialEventData> GetSpecialEvents(int weekIndex)
        {
            List<VSpecialEventData> events = new List<VSpecialEventData>();
            foreach (var phase in Phases)
            {
                if (phase.specialEventData != null)
                {
                    foreach (var eventData in phase.specialEventData)
                    {
                        if (eventData.weekIndex == weekIndex)
                        {
                            events.Add(eventData);
                        }
                    }
                }
            }
            return events;
        }
        
    }
}