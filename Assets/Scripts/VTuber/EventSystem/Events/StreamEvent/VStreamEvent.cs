using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events.DialogueEvent;

namespace VTuber.ScheduleSystem.Events
{
    public class VStreamEvent : VScheduleEvent
    {
        public int InitialTurnCount { get; private set; }
        public int TargetPopularity { get; private set; } = 0;
        public int InitialViewers { get; private set; } = 0;
        public uint SuccessEvent { get; private set; } = 0;
        public uint FailureEvent { get; private set; } = 0;
        public List<VPhaseEndingCondition> PhaseEndingConditions { get; private set; } = new List<VPhaseEndingCondition>();
        
        public VStreamEvent(VStreamEventConfiguration config) : base(config)
        {
            InitialTurnCount = config.initialTurnCount;
            TargetPopularity = config.targetPopularity;
            InitialViewers = config.initialViewers;
            SuccessEvent = config.successEvent;
            FailureEvent = config.failureEvent;
            PhaseEndingConditions = config.phaseEndingConditions;
            IsPhaseEndingEvent = config.isPhaseEndingEvent;
        }

        public List<bool> CanExecuteAsPhaseEnding(VCharacter character)
        {
            List<bool> conditionsMet = new List<bool>();
            if (!IsPhaseEndingEvent)
                return null;
            if (PhaseEndingConditions.Count == 0)
                return new List<bool>() { true };
            if (!IsExecuted)
            {
                foreach (var condition in PhaseEndingConditions)
                { 
                    conditionsMet.Add(condition.IsConditionMet(character));
                }
            }

            return conditionsMet;
        }

        public override bool Execute(VCharacter player)
        {
            if (!CanExecute(player))
                return false;
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStreamEventStart, new Dictionary<string, object>()
            {
                {"Event", this}
            });
            IsExecuted = true;
            return true;
        }

        public void SetResultEvent(bool isSuccess)
        {
            
            VDialogueEvent e;
            if (isSuccess)
            {
                e = VResourcesManager.Instance.CreateDialogueEventByID(SuccessEvent);
            }
            else
            {
                e = VResourcesManager.Instance.CreateDialogueEventByID(FailureEvent);
            }

            e.IsPhaseEndingEvent = IsPhaseEndingEvent;
            e.SetDaySchedule(_daySchedule, Coordinate);
            SetFollowUpEvent(e);
        }
    }
}