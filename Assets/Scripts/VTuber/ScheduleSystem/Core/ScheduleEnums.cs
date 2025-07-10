namespace VTuber.ScheduleSystem.Core
{
    /// <summary>
    /// 一天中的时间段
    /// </summary>
    /// 
    public enum TimeOfDay
    {
        Morning,
        Afternoon,
        Evening
    }

    /// <summary>
    /// 可安排的事件类型
    /// </summary>
    public enum ScheduleEventType
    {
        Live,
        Practice,
        Assist,
        Recovery
    }

    /// <summary>
    /// 执行失败的原因
    /// </summary>
    public enum FailureReason
    {
        InsufficientStamina,
        Other
    }

    /// <summary>
    /// 当前所处的阶段状态
    /// </summary>
    public enum PhaseState
    {
        Planning,
        Executing
    }
}