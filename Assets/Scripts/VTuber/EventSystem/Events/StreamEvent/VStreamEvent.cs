using System.Collections.Generic;
using Tutorial.Script;
using VTuber.BattleSystem.Core.KPIs;
using VTuber.BattleSystem.Effect.Conditions;
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
        
        public bool IsTutorial => _isTutorial;
        private bool _isTutorial;
        
        public List<VAttributeCondition> TutorialConditions => _tutorialConditions;
        private List<VAttributeCondition> _tutorialConditions;
        
        public List<uint> TutorialDeck => _tutorialDeck;
        private List<uint> _tutorialDeck;
        
        public Dictionary<int, List<uint>> TutorialTurnHandCards => _tutorialTurnHandCards;
        private Dictionary<int, List<uint>> _tutorialTurnHandCards;
        
        public VStreamEvent(VStreamEventConfiguration config) : base(config)
        {
            Initialize(config);
            _isTutorial = false;
        }
        
        public VStreamEvent(VTutorialStreamEventConfiguration config) : base(VDataManager.Instance.GetStreamEventConfigurationByID(config.baseEventID))
        {
            Initialize(_config as VStreamEventConfiguration);
            _isTutorial = true;
            
            _tutorialConditions = config.conditions;
            _tutorialDeck = config.deck;
            _tutorialTurnHandCards = config.turnHandCards;
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