using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.BattleSystem.Core.ScriptSystem;
using VTuber.Character;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.Core.ScriptSystem;
using VTuber.EventSystem.Events;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.UI;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.ScheduleSystem.Events
{
    public class VScheduleEventSaveData
    {
        public Vector2Int coordinate;
        public VScheduleEventSaveData followUpEventSaveData;
        public uint id;
        public bool isExecuted;
        public bool isFollowUp;
        public bool isPhaseEndingEvent;
        public bool isPhaseStart;
        public bool isSpecialEvent;
        public bool isStream;
        public int phase;
    }

    public struct VScheduleEventSlotCoopEffectData
    { 
        public Sprite pfp;
        public List<VRaisingEffect> effects;
        public string description;

        public VScheduleEventSlotCoopEffectData(Sprite icon, List<VRaisingEffect> coopEffects, string description)
        {
            pfp = icon;
            effects = coopEffects;
            this.description = description;
        }

        public void ApplyEffect(VCharacter character)
        {
            List<string> descriptions = description.Split("|").ToList();
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].ApplyEffect(character, null, VAnimationRequestFactory.Create(VInstigatorType.Event, pfp, descriptions[i]));
            }
        }
    }

    public class VScheduleEvent
    {
        protected readonly VScheduleEventConfiguration _config;

        protected VDaySchedule _daySchedule;
        protected VScheduleEvent _followUpEvent;

        private bool _isSchedulingConditionMet;

        public bool isFollowUp;

        public uint EventID => _config.id;
        public string EventName => _config.eventName;
        public string Description => _config.description;
        public VEventType Type => _config.type;
        public Sprite Icon { get; protected set; }
        public Color BackgroundColor => _config.backgroundColor;
        public VEventCostType CostType => _config.costType;

        public int Cost => _config.cost;

        //adding duration to meet event may last across 2 times period
        public int Duration => _config.Duration;

        public Vector2Int Coordinate { get; protected set; } = new(-1, -1);

        public bool IsExecuted { get; protected set; }

        public VDaySchedule DaySchedule => _daySchedule;
        public VPhase Phase { get; set; }
        public bool IsSpecialEvent { get; set; }
        public bool IsPhaseStart { get; set; }
        public bool IsPhaseEndingEvent { get; set; }

        public VScheduleEvent FollowUpEvent => _followUpEvent;
        
        public Dictionary<VScheduleSlot, VScheduleEventSlotCoopEffectData> CoopEffects { get; }

        public List<VPlacingCondition> PlacingConditions { get; }

        public VSchedulingCondition SchedulingCondition { get; }

        
        public VScheduleEvent(VScheduleEventConfiguration config)
        {
            _config = config;
            CoopEffects = new ();

            PlacingConditions = new List<VPlacingCondition>();
            foreach (var conditionId in config.placingConditions)
                PlacingConditions.Add(VDataManager.Instance.GetPlacingCondtionByID(conditionId));
            SchedulingCondition = config.schedulingCondition;
            Icon = VResourcesManager.Instance.TryGetSprite(_config.icon);
        }

        public void SetSchedulingConditionMet(bool value)
        {
            _isSchedulingConditionMet = value;
        }

        public void SetCoopEffects(VScheduleSlot slot, List<VRaisingEffect> coopEffects, Sprite icon, string description)
        {
            CoopEffects[slot] = new VScheduleEventSlotCoopEffectData(icon, coopEffects, description);
        }

        public void RemoveCoopEffects(VScheduleSlot slot)
        {
            CoopEffects.Remove(slot);
        }

        public void SetDaySchedule(VDaySchedule daySchedule, Vector2Int position)
        {
            _daySchedule = daySchedule;
            Coordinate = position;
        }

        public void SetFollowUpEvent(VScheduleEvent followUpEvent)
        {
            _followUpEvent = followUpEvent;
        }

        public void SetDuration(int duration)
        {
            _config.SetDuration(duration);
        }

        public virtual bool CanExecute(VCharacter player)
        {
            return true;
        }

        public virtual bool Execute(VCharacter player)
        {
            if (!CanExecute(player))
                return false;
            IsExecuted = true;
            return true;
        }

        public void Reset()
        {
            _daySchedule = null;
            Coordinate = new Vector2Int(-1, -1);
        }

        public void AdvanceTime()
        {
            if (_daySchedule is not null)
                _daySchedule.OnEventExecuted(this);
        }

        public void AddFollowUpEvent(VEventType eventType, uint id)
        {
            if (_followUpEvent is null)
            {
                _followUpEvent = VDataManager.Instance.CreateEvent(eventType, id);
                _followUpEvent._daySchedule = _daySchedule;
                _followUpEvent.isFollowUp = true;
            }
            else
            {
                var followUp = _followUpEvent;
                while (followUp._followUpEvent is not null) followUp = followUp.FollowUpEvent;
                followUp._followUpEvent = VDataManager.Instance.CreateEvent(eventType, id);
                followUp._followUpEvent._daySchedule = followUp._daySchedule;
                followUp._followUpEvent.isFollowUp = true;
            }
        }

        public void ExecuteEffectsBeforeEvent(VCharacter character)
        {
            if (_isSchedulingConditionMet && SchedulingCondition.ShouldExecuteBeforeEvent)
                foreach (var effect in SchedulingCondition.Effects)
                    effect?.ApplyEffect(character, null, VAnimationRequestFactory.Create(VInstigatorType.Event, Icon, Description));
        }
        
        public void ExecuteAppendedEffects(VCharacter character)
        {
            if (CoopEffects is not null && character is not null)
                foreach (var data in CoopEffects.Values)
                {
                    data.ApplyEffect(character);
                }

            if (_isSchedulingConditionMet && !SchedulingCondition.ShouldExecuteBeforeEvent)
                foreach (var effect in SchedulingCondition.Effects)
                    effect?.ApplyEffect(character, null, VAnimationRequestFactory.Create(VInstigatorType.Event, Icon, Description));
        }

        public void SetExecuted()
        {
            IsExecuted = true;
        }

        public VScheduleEventSaveData Save(VScript script)
        {
            return new VScheduleEventSaveData
            {
                id = EventID,
                isStream = this is VStreamEvent,
                coordinate = Coordinate,
                isExecuted = IsExecuted,
                isFollowUp = isFollowUp,
                followUpEventSaveData = _followUpEvent?.Save(script),
                isSpecialEvent = IsSpecialEvent,
                isPhaseStart = IsPhaseStart,
                isPhaseEndingEvent = IsPhaseEndingEvent,
                phase = Phase is null ? -1 : script.GetPhaseIndex(Phase)
            };
        }

        public static VScheduleEvent Load(VScheduleEventSaveData saveData, VScript script)
        {
            VScheduleEventConfiguration config;
            if (saveData.isStream)
                config = VDataManager.Instance.GetStreamEventConfigurationByID(saveData.id);
            else
                config = VDataManager.Instance.GetDialogueEventConfigurationByID(saveData.id);
            var eventInstance = config.CreateEvent();
            eventInstance.Coordinate = saveData.coordinate;
            eventInstance.IsExecuted = saveData.isExecuted;
            eventInstance.isFollowUp = saveData.isFollowUp;
            eventInstance.IsSpecialEvent = saveData.isSpecialEvent;
            eventInstance.IsPhaseStart = saveData.isPhaseStart;
            eventInstance.IsPhaseEndingEvent = saveData.isPhaseEndingEvent;
            if (saveData.phase != -1)
                eventInstance.Phase = script.GetPhase(saveData.phase);
            if (saveData.followUpEventSaveData is not null)
                eventInstance._followUpEvent = Load(saveData.followUpEventSaveData, script);
            return eventInstance;
        }
    }
}