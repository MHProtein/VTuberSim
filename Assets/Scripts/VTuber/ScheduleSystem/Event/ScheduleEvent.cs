using UnityEngine;
using VTuber.ScheduleSystem.Core;

namespace VTuber.ScheduleSystem.Event
{
    /// <summary>
    /// 运行时事件类，由配置生成，包含执行逻辑
    /// </summary>
    public class ScheduleEvent
    {
        public string EventName => _config.eventName;
        public string Description => _config.description;
        public ScheduleEventType Type => _config.type;
        public Sprite Icon => _config.icon;
        public int StaminaCost => _config.staminaCost;

        private readonly ScheduleEventConfiguration _config;

        public ScheduleEvent(ScheduleEventConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// 判断玩家状态是否允许执行
        /// </summary>
        public bool CanExecute(PlayerStatus player)
        {
            return player.Stamina >= _config.staminaCost;
        }

        /// <summary>
        /// 执行事件逻辑
        /// </summary>

        public bool Execute(PlayerStatus player)
        {
            if (!CanExecute(player))
            {
                Debug.LogWarning($"无法执行事件：{EventName}，体力不足");
                return false;
            }

            player.Stamina -= _config.staminaCost;
            player.Experience += _config.skillExpBonus;

            return true;
        }
    }
}