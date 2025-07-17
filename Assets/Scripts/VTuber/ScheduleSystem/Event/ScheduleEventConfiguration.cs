using UnityEngine;
using VTuber.ScheduleSystem.Core;

namespace VTuber.ScheduleSystem.Event
{
    [CreateAssetMenu(fileName = "ScheduleEventConfig", menuName = "VTuber/Schedule/Event")]
    public class ScheduleEventConfiguration : ScriptableObject
    {
        public string eventName;

        [TextArea]
        public string description;

        public Sprite icon;

        public ScheduleEventType type;

        public int staminaCost = 10;

        // 可拓展：经验奖励、资源奖励、概率失败等
        public int moodBonus = 0;
        public int skillExpBonus = 0;

        public ScheduleEvent CreateEvent()
        {
            return new ScheduleEvent(this);
        }
    }
}