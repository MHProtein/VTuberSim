using UnityEngine;
using VTuber.ScheduleSystem.Core;

namespace VTuber.ScheduleSystem.Events
{
    [CreateAssetMenu(fileName = "ScheduleEventConfig", menuName = "VTuber/Schedule/Event")]
    public class VScheduleEventConfiguration : ScriptableObject
    {
        public string eventName;
        
        [Range(1, 3)]
        public int duration = 1;
        
        public int Duration => duration;
        
        
        [TextArea]
        public string description;

        public Sprite icon;
        
        public Color backgroundColor = Color.white;

        public ScheduleEventType type;

        public int staminaCost = 10;

        // 可拓展：经验奖励、资源奖励、概率失败等
        public int moodBonus = 0;
        public int skillExpBonus = 0;

        public virtual VScheduleEvent CreateEvent()
        {
            return new VScheduleEvent(this);
        }
    }
}