using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events.DialogueEvent;

namespace VTuber.ScheduleSystem.Events
{
    public class VStreamEvent : VDialogueEvent
    {
        public int InitialTurnCount { get; private set; }
        public int TargetPopularity { get; private set; } = 0;
        public int InitialViewers { get; private set; } = 0;
        public int SuccessEvent { get; private set; } = 0;
        public int FailureEvent { get; private set; } = 0;
        
        public int MainAttributeIndex { get; private set; }
        
        public List<int> AbilityTurnCounts { get; private set; }
        
        public List<VPhaseEndingCondition> PhaseEndingConditions { get; private set; } = new List<VPhaseEndingCondition>();
        
        public VStreamEvent(VStreamEventConfiguration config) : base(config)
        {
            InitialTurnCount = config.initialTurnCount;
            TargetPopularity = config.targetPopularity;
            InitialViewers = config.initialViewers;
            MainAttributeIndex = config.mainAttributeIndex;
            AbilityTurnCounts = config.abilityTurnCounts;
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

        public void SetResultEvent(bool isSuccess)
        {
            VDialogueEvent e;
            if (isSuccess)
            {
                if (SuccessEvent == -1)
                    return;
                e = VResourcesManager.Instance.CreateDialogueEventByID((uint)SuccessEvent);
            }
            else
            {
                if (FailureEvent == -1)
                    return;
                e = VResourcesManager.Instance.CreateDialogueEventByID((uint)FailureEvent);
            }

            e.isFollowUp = true;
            e.IsPhaseEndingEvent = IsPhaseEndingEvent;
            e.SetDaySchedule(_daySchedule, Coordinate);
            var temp = FollowUpEvent;
            _followUpEvent = e;
            _followUpEvent.SetFollowUpEvent(temp);
        }
    }
}