using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.Core.Foundation;

namespace VTuber.CoopSystem
{
    [Serializable]
    public struct VCoopLevel
    {
        [HorizontalGroup] public int from;
        [HorizontalGroup] public int to;
        [HorizontalGroup] public string levelName;

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
        
        [Header("协助者")]
        [SerializeField] private uint id;
        [FormerlySerializedAs("name")] [SerializeField] private string cooperatorName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private List<VCoopLevel> _coopLevels;
        
        [Header("帮助事件")]
        [HorizontalGroup("MinMaxEvents")]
        [SerializeField] private int minEvents;
        [Header("")]
        [HorizontalGroup("MinMaxEvents")]
        [SerializeField] private int maxEvents;
        [SerializeField] private List<float> _dayTimeProbabilities;
        [SerializeField] private List<float> _dayProbabilities;
        [SerializeField] private List<uint> _coopEvents;

        protected override void Awake()
        {
            base.Awake();
            _dayTimeProbabilities = new List<float>();
            for (int i = 0; i < 3; i++)
            {
                _dayTimeProbabilities.Add(0.33f);
            }
            _dayProbabilities = new List<float>();
            for (int i = 0; i < 7; i++)
            {
                _dayProbabilities.Add(0.14f);
            }
        }
    }
}