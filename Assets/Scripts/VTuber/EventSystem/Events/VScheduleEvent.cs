using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core.ScriptSystem;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Schedule;

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
        public VScheduleEventType Type => _config.type;
        public string Icon => _config.icon;
        public Color BackgroundColor => _config.backgroundColor;
        
        public VEventCostType CostType => _config.costType;
        public int Cost => _config.cost;
        //adding duration to meet event may last across 2 times period
        public int Duration => _config.Duration;
        
        protected readonly VScheduleEventConfiguration _config;

        public Vector2Int Coordinate { get; protected set; } = new Vector2Int(-1, -1);
        
        public bool IsExecuted { get; protected set; } = false;
        
        public DaySchedule DaySchedule => _daySchedule;
        public VPhase Phase { get; set; }
        public bool IsPhaseStart { get; set; } = false;
        public bool IsPhaseEndingEvent { get; protected set; } = false;

        protected DaySchedule _daySchedule;

        public VScheduleEvent(VScheduleEventConfiguration config)
        {
            _config = config;
        }
        
        public void SetDaySchedule(DaySchedule daySchedule, Vector2Int position)
        {
            _daySchedule = daySchedule;
            Coordinate = position;
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

        public virtual VScheduleEvent GetNextEvent()
        {
            return _daySchedule.NextEvent();
        }

        public void Reset()
        {
            _daySchedule = null;
            Coordinate = new Vector2Int(-1, -1);
        }
    }
}