using System.Collections.Generic;
using UnityEngine;
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
        public string EventName => _config.eventName;
        public string Description => _config.description;
        public ScheduleEventType Type => _config.type;
        public string Icon => _config.icon;
        
        public VEventCostType CostType => _config.costType;
        public int Cost => _config.cost;
        //adding duration to meet event may last across 2 times period
        public int Duration => _config.Duration;
        
        protected readonly VScheduleEventConfiguration _config;

        public bool IsExecuted { get; protected set; } = false;
        
        private DaySchedule _daySchedule;

        public VScheduleEvent(VScheduleEventConfiguration config)
        {
            _config = config;
        }
        
        public void SetDaySchedule(DaySchedule daySchedule)
        {
            _daySchedule = daySchedule;
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
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStreamEventStart, new Dictionary<string, object>()
            {
                {"Event", this}
            });
            IsExecuted = true;
            return true;
        }

        public virtual void NextEvent()
        {
            _daySchedule.NextEvent();
        }
    }
}