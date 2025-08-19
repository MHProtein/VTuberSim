using System.Collections.Generic;
using VTuber.BattleSystem.Core.KPIs;
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
        
        public int ExtraTargetPopularity { get; private set; }
        public int AbilityBonus { get; private set; }
        
        public int MainAttributeIndex { get; private set; }
        
        public List<int> AbilityTurnCounts { get; private set; }
        
        public List<VKPI> Kpis { get; private set; }
        
        public VStreamEvent(VStreamEventConfiguration config) : base(config)
        {
            InitialTurnCount = config.initialTurnCount;
            TargetPopularity = config.targetPopularity;
            InitialViewers = config.initialViewers;
            MainAttributeIndex = config.mainAttributeIndex;
            AbilityTurnCounts = config.abilityTurnCounts;
            SuccessEvent = config.successEvent;
            FailureEvent = config.failureEvent;
            IsPhaseEndingEvent = config.isPhaseEndingEvent;
            ExtraTargetPopularity = config.extraTargetPopularity;
            AbilityBonus = config.attributeBonus;
            Kpis = config.kpis;
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