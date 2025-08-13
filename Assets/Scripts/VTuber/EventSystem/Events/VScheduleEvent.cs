using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VTuber.BattleSystem.Core.ScriptSystem;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.EventSystem.Events;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.UI;

namespace VTuber.ScheduleSystem.Events
{
    /// <summary>
    /// 运行时事件类，由配置生成，包含执行逻辑
    /// </summary>
    public class VScheduleEvent
    {
        public uint EventID => _config.id;
        public string EventName => _config.eventName;
        public string Description => _config.description;
        public VEventType Type => _config.type;
        public string Icon => _config.icon;
        public Color BackgroundColor => _config.backgroundColor;
        
        public VEventCostType CostType => _config.costType;
        public int Cost => _config.cost;
        //adding duration to meet event may last across 2 times period
        public int Duration => _config.Duration;
        
        protected readonly VScheduleEventConfiguration _config;

        public Vector2Int Coordinate { get; protected set; } = new Vector2Int(-1, -1);
        
        public bool IsExecuted { get; protected set; } = false;
        
        public VDaySchedule DaySchedule => _daySchedule;
        public VPhase Phase { get; set; }
        public bool IsSpecialEvent { get; set; } = false;
        public bool IsPhaseStart { get; set; } = false;
        public bool IsPhaseEndingEvent { get; set; } = false;

        protected VDaySchedule _daySchedule;

        public VScheduleEvent FollowUpEvent => _followUpEvent;
        protected VScheduleEvent _followUpEvent;

        public Dictionary<VScheduleSlot, List<VRaisingEffect>> CoopEffects => _coopEffects;
        private Dictionary<VScheduleSlot, List<VRaisingEffect>> _coopEffects;
        
        public List<VPlacingCondition> PlacingConditions => _placingConditions;
        private List<VPlacingCondition> _placingConditions;

        public bool isFollowUp = false;
        
        public VScheduleEvent(VScheduleEventConfiguration config)
        {
            _config = config;
            _coopEffects = new Dictionary<VScheduleSlot, List<VRaisingEffect>>();

            _placingConditions = new List<VPlacingCondition>();
            foreach (var conditionId in config.placingConditions)
            {
                _placingConditions.Add(VResourcesManager.Instance.GetPlacingCondtionByID(conditionId));
            }
        }
        
        public void SetCoopEffects(VScheduleSlot slot, List<VRaisingEffect> coopEffects)
        {
            _coopEffects[slot] = coopEffects;
        }
        
        public void RemoveCoopEffects(VScheduleSlot slot)
        {
            _coopEffects[slot] = null;
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

        /// <summary>
        /// 判断玩家状态是否允许执行
        /// </summary>
        public virtual bool CanExecute(VCharacter player)
        {
            return true;
        }

        /// <summary>
        /// 执行事件逻辑
        /// </summary>

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
            if(_daySchedule is not null)
                _daySchedule.OnEventExecuted(this);
        }

        public void AddFollowUpEvent(VEventType eventType, uint id)
        {
            if (_followUpEvent is null)
            {
                _followUpEvent = VResourcesManager.Instance.CreateEvent(eventType, id);
                _followUpEvent._daySchedule = _daySchedule;
                _followUpEvent.isFollowUp = true;
            }
            else
            {
                var followUp = _followUpEvent;
                while (followUp._followUpEvent is not null)
                {
                    followUp = followUp.FollowUpEvent;
                }
                followUp._followUpEvent = VResourcesManager.Instance.CreateEvent(eventType, id);
                followUp._followUpEvent._daySchedule = followUp._daySchedule;
                followUp._followUpEvent.isFollowUp = true;
            }
        }

        public void ExecuteCoopEvents(VCharacter character)
        {
            if(CoopEffects is not null && character is not null)
                foreach (var effect in CoopEffects.Values)
                {
                    effect?.ForEach(x => x.ApplyEffect(character));
                }
        }
    }
}