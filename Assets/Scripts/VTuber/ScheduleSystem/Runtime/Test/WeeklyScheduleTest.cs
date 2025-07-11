using UnityEngine;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Event;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.Runtime;

public class ScheduleTestRunner : MonoBehaviour
{
    void Start()
    {
        Debug.Log("<color=cyan>开始模拟一周计划测试</color>");

        // 初始化测试玩家状态
        var player = new PlayerStatus
        {
            Stamina = 100,
            Experience = 0
        };

        var weeklySchedule = new WeeklySchedule();

        // 安排每天三段时间的事件
        for (int day = 0; day < 7; day++)
        {
            weeklySchedule.SetEvent(day, TimeOfDay.Morning, new ScheduleEvent(CreateConfig(ScheduleEventType.Live, 10, 5)));
            weeklySchedule.SetEvent(day, TimeOfDay.Afternoon, new ScheduleEvent(CreateConfig(ScheduleEventType.Practice, 8, 6)));
            weeklySchedule.SetEvent(day, TimeOfDay.Evening, new ScheduleEvent(CreateConfig(ScheduleEventType.Recovery, 0, 0)));
        }

        // 执行排程，每天输出阶段后的体力与经验
        for (int day = 0; day < 7; day++)
        {
            Debug.Log($"<color=yellow>Day {day + 1}</color>");
            for (int t = 0; t < 3; t++)
            {
                var time = (TimeOfDay)t;
                var ev = weeklySchedule.GetEvent(day, time);

                ev?.Execute(player);  // 确保执行状态改变

                Debug.Log($"阶段：{time} | 体力: {player.Stamina}, 经验: {player.Experience}");
            }
        }

        Debug.Log($"<color=green>模拟结束</color>\n最终体力: {player.Stamina}\n最终经验: {player.Experience}");
    }

    private ScheduleEventConfiguration CreateConfig(ScheduleEventType type, int staminaCost, int skillExpBonus)
    {
        var config = ScriptableObject.CreateInstance<ScheduleEventConfiguration>();
        config.eventName = type.ToString();
        config.type = type;
        config.staminaCost = staminaCost;
        config.skillExpBonus = skillExpBonus;
        return config;
    }
}