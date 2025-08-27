using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Core;

namespace VTuber.CoopSystem
{
    [Serializable]
    public class VCoopLevel
    {
        [HorizontalGroup] public int from;
        [HorizontalGroup] public int to;
        [HorizontalGroup] public string levelName;
        public VEventType eventType;
        public uint upgradeEventID;
        public VCoopLevel nextLevel;

        public bool InLevel(int value)
        {
            return value >= from && value <= to;
        }
    }

    public class VCooperatorConfiguration : VScriptableObject
    {
        public uint Id => id;
        public string Name => cooperatorName;
        public string Description => description;
        public Sprite Icon => icon;
        public List<VCoopLevel> CoopLevels => _coopLevels;
        public int MinEvents => minEvents;
        public int MaxEvents => maxEvents;
        public List<float> DayTimeProbabilities => _dayTimeProbabilities;
        public List<float> DayProbabilities => _dayProbabilities;
        public List<uint> CoopEvents => _coopEvents;
        
        public uint BaseCoopEvent => baseCoopEvent;
        
        [Header("协助者")]
        [SerializeField] private uint id;
        [FormerlySerializedAs("name")] [SerializeField] [LabelText("协助者名称")] private string cooperatorName;
        [SerializeField] [LabelText("协助者描述")] private string description;
        [SerializeField] [LabelText("协助者图标")] private Sprite icon;
        [SerializeField] [LabelText("基础协助事件")] private uint baseCoopEvent;
        [SerializeField] [LabelText("协助者等级")] private List<VCoopLevel> _coopLevels;

        [Header("帮助事件")]
        [HorizontalGroup("MinMaxEvents")]
        [SerializeField] [LabelText("最少帮助事件")] private int minEvents;
        [Header("")]
        [HorizontalGroup("MinMaxEvents")]
        [SerializeField] [LabelText("最多帮助事件")] private int maxEvents;
        [SerializeField] [LabelText("时间概率")] private List<float> _dayTimeProbabilities;
        [SerializeField] [LabelText("周X概率")] private List<float> _dayProbabilities;
        [SerializeField] [LabelText("协助事件")] private List<uint> _coopEvents;
    }
}