using System.Collections.Generic;
using Tutorial.Script;
using VTuber.BattleSystem.Core.KPIs;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Events.DialogueEvent;

namespace VTuber.ScheduleSystem.Events
{
    public class VStreamEvent : VDialogueEvent
    {
        public int InitialTurnCount { get; private set; }
        public int TargetPopularity { get; private set; }
        public int InitialViewers { get; private set; }
        public int SuccessEvent { get; private set; }
        public int FailureEvent { get; private set; }

        public int ExtraTargetPopularity { get; private set; }
        public int AbilityBonus { get; private set; }

        public int MainAttributeIndex { get; private set; }

        public List<int> AbilityTurnCounts { get; private set; }

        public List<VKPI> Kpis { get; private set; }

        public bool IsTutorial { get; }

        public List<VAttributeCondition> TutorialConditions { get; }

        public List<uint> TutorialDeck { get; }

        public Dictionary<int, List<uint>> TutorialTurnHandCards { get; }
        public VTipConfig TutorialTipConfig { get; }

        public VStreamEvent(VStreamEventConfiguration config) : base(config)
        {
            Initialize(config);
            IsTutorial = false;
        }

        public VStreamEvent(VTutorialStreamEventConfiguration config) : base(
            VDataManager.Instance.GetStreamEventConfigurationByID(config.baseEventID))
        {
            Initialize(_config as VStreamEventConfiguration);
            IsTutorial = true;

            TutorialConditions = config.conditions;
            TutorialDeck = config.deck;
            TutorialTurnHandCards = config.turnHandCards;
            TutorialTipConfig = config.tip;
        }
        
        private void Initialize(VStreamEventConfiguration config)
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
                e = VDataManager.Instance.CreateDialogueEventByID((uint)SuccessEvent);
            }
            else
            {
                if (FailureEvent == -1)
                    return;
                e = VDataManager.Instance.CreateDialogueEventByID((uint)FailureEvent);
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