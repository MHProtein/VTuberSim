using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.ScheduleSystem.Core;

namespace VTuber.BattleSystem.Core.ScriptSystem
{
    [Serializable]
    public class VSpecialEventData
    {
        public int weekIndex;
        public int dayIndex;
        public TimeOfDay timeOfDay;
        public ScheduleEventType eventType;
        public uint eventID;
    }
    [Serializable]
    public class VPhase
    {
        [SerializeField] public string phaseName;
        [SerializeField] public string description;
        
        [HorizontalGroup("开始事件", Gap = 10)]
        [Header("开始事件")]
        [LabelText("周")]
        public int startEventWeekIndex;

        [HorizontalGroup("开始事件")]
        [Header("")]
        [LabelText("天")]
        public int startEventDayIndex;
        
        [HorizontalGroup("开始事件")]
        [Header("")]
        [LabelText("时间段")]
        public TimeOfDay startEventTimeOfDay;
        [SerializeField] public uint startEventID;
        
        [HorizontalGroup("结束事件", Gap = 10)]
        [Header("结束事件")]
        [LabelText("周")]
        public int endEventWeekIndex;

        [HorizontalGroup("结束事件")]
        [Header("")]
        [LabelText("天")]
        public int endEventDayIndex;
        
        [HorizontalGroup("结束事件")]
        [Header("")]
        [LabelText("时间段")]
        public TimeOfDay endEventTimeOfDay;
        
        [LabelText("结束事件ID")][SerializeField] List<uint> endEventIDs = new List<uint>();
        
        
        [LabelText("特殊事件")]
        [SerializeField] public List<VSpecialEventData> specialEventData;

    }
}