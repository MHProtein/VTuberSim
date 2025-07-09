using UnityEngine;

namespace VTuber.ScheduleSystem.Runtime
{
    /// <summary>
    /// 玩家当前状态（临时），未来可对接 Character 模块
    /// </summary>
    public class PlayerStatus
    {
        /// <summary>
        /// 当前体力值
        /// </summary>
        public int Stamina { get; private set; }

        /// <summary>
        /// 最大体力值（可扩展）
        /// </summary>
        public int MaxStamina { get; private set; } = 100;

        public PlayerStatus(int stamina)
        {
            Stamina = stamina;
        }

        /// <summary>
        /// 消耗体力，返回是否成功
        /// </summary>
        public bool ConsumeStamina(int amount)
        {
            if (Stamina >= amount)
            {
                Stamina -= amount;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 恢复体力
        /// </summary>
        public void RestoreStamina(int amount)
        {
            Stamina = Mathf.Min(Stamina + amount, MaxStamina);
        }
    }
}