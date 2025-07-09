using System.Collections.Generic;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.Event;
using VTuber.ScheduleSystem.Runtime;
using VTuber.ScheduleSystem.Phase;
using VTuber.Core.Foundation;


namespace VTuber.ScheduleSystem.Runtime
{
    /// <summary>
    /// 用于调度并执行一整周的排程逻辑
    /// </summary>
    public class ScheduleExecutor
    {
        private WeeklySchedule _weeklySchedule;
        private PlayerStatus _playerStatus;

        public ScheduleExecutor(WeeklySchedule schedule, PlayerStatus player)
        {
            _weeklySchedule = schedule;
            _playerStatus = player;
        }

        /// <summary>
        /// 执行一整周所有阶段安排
        /// </summary>
        public void ExecuteAll()
        {
            for (int day = 0; day < 7; day++)
            {
                ExecuteDay(day);
            }
        }

        /// <summary>
        /// 执行指定天的三个阶段
        /// </summary>
        public void ExecuteDay(int dayIndex)
        {
            foreach (TimeOfDay time in System.Enum.GetValues(typeof(TimeOfDay)))
            {
                var scheduleEvent = _weeklySchedule.GetEvent(dayIndex, time);
                var phase = new PhaseData(dayIndex, time, scheduleEvent);

                ExecutePhase(phase);
            }
        }

        /// <summary>
        /// 执行单个阶段的安排（如早上）
        /// </summary>
        public void ExecutePhase(PhaseData phase)
        {
            var evt = phase.Event;

            if (evt == null)
            {
                VDebug.Log($"阶段无事件安排：{phase}");
                return;
            }

            if (evt.CanExecute(_playerStatus))
            {
                evt.Execute(_playerStatus);
                VDebug.Log($"已执行阶段事件：{phase}");
            }
            else
            {
                VDebug.LogWarning($"无法执行阶段事件（条件不符或体力不足）：{phase}");
            }
        }
    }
}