using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Event;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.Runtime;

public class ScheduleTestRunnerUI : MonoBehaviour
{
    public Transform eventButtonContainer;
    public GameObject eventButtonPrefab;
    public List<ScheduleEventConfiguration> eventConfigs;

    private WeeklySchedule weeklySchedule;
    private PlayerStatus player;

    private int currentDay = 0;
    private bool scheduleComplete = false;

    void Start()
    {
        Debug.Log("<color=cyan>进入交互式计划排程模式</color>");

        // 初始化状态
        player = new PlayerStatus { Stamina = 100, Experience = 0 };
        weeklySchedule = new WeeklySchedule();

        // 创建事件按钮
        foreach (var config in eventConfigs)
        {
            var go = Instantiate(eventButtonPrefab, eventButtonContainer);
            var buttonScript = go.GetComponent<UIEventButton>();
            buttonScript.Setup(config, OnEventButtonClicked);
        }
    }

    void OnEventButtonClicked(ScheduleEventConfiguration config)
    {
        if (scheduleComplete)
        {
            Debug.LogWarning("🛑 一周排程已完成，不能再安排事件！");
            return;
        }

        while (currentDay < 7)
        {
            var scheduled = TryScheduleEvent(config);

            if (scheduled)
                return;

            currentDay++;
            if (currentDay < 7)
                Debug.Log($"📅 当前天已排满，切换到第 {currentDay + 1} 天");
        }

        // 所有天都满了
        Debug.Log("✅ <color=green>本周排程完成，开始执行事件！</color>");
        scheduleComplete = true;
        ExecuteSchedule();
    }

    private bool TryScheduleEvent(ScheduleEventConfiguration config)
    {
        var scheduleEvent = new ScheduleEvent(config);

        foreach (TimeOfDay time in System.Enum.GetValues(typeof(TimeOfDay)))
        {
            if (weeklySchedule.CanScheduleEvent(currentDay, time, config.duration))
            {
                weeklySchedule.SetEvent(currentDay, time, scheduleEvent);
                Debug.Log($"✅ <color=green>安排事件：</color>【{config.eventName}】 → 第 {currentDay + 1} 天 {time}（持续 {config.duration}）");
                return true;
            }
        }

        return false;
    }

    private void ExecuteSchedule()
    {
        for (int day = 0; day < 7; day++)
        {
            Debug.Log($"<color=yellow>Day {day + 1}</color>");
            for (int t = 0; t < 3; t++)
            {
                var time = (TimeOfDay)t;
                var ev = weeklySchedule.GetEvent(day, time);

                if (weeklySchedule.GetDay(day).IsPrimary(time))
                {
                    ev?.Execute(player);
                }

                Debug.Log($"阶段：{time} | 体力: {player.Stamina}, 经验: {player.Experience}");
            }
        }
    }

    public void NextDay()  // 保留手动跳天功能
    {
        if (currentDay < 6)
        {
            currentDay++;
            Debug.Log($"📅 手动切换到第 {currentDay + 1} 天");
        }
        else
        {
            Debug.Log("🚫 已经是最后一天");
        }
    }
}
