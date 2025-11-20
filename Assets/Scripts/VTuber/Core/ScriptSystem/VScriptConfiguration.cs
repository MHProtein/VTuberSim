using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.BattleSystem.Core.KPIs;
using VTuber.BattleSystem.Core.ScriptSystem;
using VTuber.Character.Attributes;
using VTuber.CoopSystem;
using VTuber.Core.Foundation;

namespace VTuber.Core.ScriptSystem
{
    [Serializable]
    public struct VScoreLevel
    {
        [HorizontalGroup("ScoreLevel", Gap = 10)]
        public int low;

        [HorizontalGroup("ScoreLevel", Gap = 10)]
        public int high;

        [HorizontalGroup("ScoreLevel", Gap = 10)]
        public string name;

        public bool InLevel(int score)
        {
            if (high == -1)
                return score >= low;
            return score >= low && score <= high;
        }
    }

    public class VScriptConfiguration : VScriptableObject
    {
        public int index;
        public string scriptName;
        public string description;
        public Sprite icon;
        public List<VCooperatorConfiguration> coops;
        [Header("基础卡牌")] [SerializeField] public Dictionary<string, List<uint>> cardIDs;

        [Space(5)] [Header("初始事件")] [SerializeField]
        public List<uint> eventIDs;

        [Space(5)] [Header("初始直播事件")] [SerializeField]
        public List<uint> streamEventIDs;

        [Space(5)] [Header("评分")] [SerializeField] [LabelText("总面板系数")]
        public float abilityCoefficient = 1f;

        [SerializeField] [LabelText("粉丝数系数")] public float followerCoefficient = 1f;

        [SerializeField] [LabelText("最高舰长数系数")]
        public float membershipCoefficient = 1f;

        [SerializeField] [LabelText("直播热度系数")] public List<VRangeValueMap<float>> popularityCoefficient;
        [SerializeField] [LabelText("通过加成")] public int successBonus;

        [SerializeField] public List<VScoreLevel> scoreLevels;
        [SerializeField] public List<VKPIInfo> kpis;

        [Space(5)] [Header("阶段")] [SerializeField]
        public List<VPhase> phases;

        public uint staminaNotEnoughEventID = 8;
    }
}