using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Core.Foundation;

namespace VTuber.Reincarnation
{
    [Serializable]
    public class VCardRarityRequirement
    {
        [Space(3)] [HorizontalGroup("VCardRarityRequirement")] [LabelText("稀有度")]
        public VCardRarity rarity;

        [HorizontalGroup("VCardRarityRequirement")] [LabelText("数量")]
        public int count;
    }

    [Serializable]
    public class VEffectLevelRequirement
    {
        [Space(3)] [HorizontalGroup("VEffectLevelRequirement")] [LabelText("等级")]
        public int level;

        [HorizontalGroup("VEffectLevelRequirement")] [LabelText("最高等级")]
        public int highestLevel;

        [HorizontalGroup("VEffectLevelRequirement")] [LabelText("数量")]
        public int count;
    }

    [Serializable]
    public class VCardLevelInfo
    {
        [Space(3)] [LabelText("继承位数量")] public int cardCount;

        [LabelText("容量")] public int cardTotalCapacity;
        [LabelText("稀有度要求")] public List<VCardRarityRequirement> cardRarityRequirements = new();
    }

    [Serializable]
    public class VCardCapacityInfo
    {
        [Space(3)] [HorizontalGroup("cardCapacity")] [LabelText("稀有度")]
        public VCardRarity rarity;

        [HorizontalGroup("cardCapacity")] [LabelText("容量")]
        public int capacity;

        [HorizontalGroup("cardCapacity")] [LabelText("升级容量")]
        public int upgradeCapacity;
    }

    [Serializable]
    public class VRelicRewardInfo
    {
        [Space(3)] [HorizontalGroup("VRelicRewardInfo")] [LabelText("事件ID")]
        public uint eventID;

        [HorizontalGroup("VRelicRewardInfo")] [LabelText("遗物ID")]
        public List<uint> relicIDs;
    }

    [Serializable]
    public class VAttributeLevelInfo
    {
        [Space(3)] [HorizontalGroup("VRelicRewardInfo")] [LabelText("等级参数")]
        public string levelParam;
    }

    [Serializable]
    public class VAbilityEffectInfo : VAttributeEffectInfo
    {
        [Space(3)] [LabelText("能力ID（歌力..）")] public int ability;

        [LabelText("是否是获得效率")] public bool isGainEfficiency;
    }

    [Serializable]
    public class VAttributeEffectInfo
    {
        [Space(3)] [LabelText("效果ID")] public uint effect;

        [LabelText("等级信息")] public List<VAttributeLevelInfo> levelInfos;
    }

    [Serializable]
    public class VEffectLevelInfo
    {
        [Space(3)] [LabelText("数量")] public int count;

        [LabelText("容量")] public int capacity;

        [Header("直播属性")] [LabelText("直播属性要求")] public List<VEffectLevelRequirement> streamEffectsRequirements;

        [Header("其他属性")] [LabelText("其他属性要求")] public List<VEffectLevelRequirement> attributeEffectsRequirements;
    }

    public class VReincarnationConfiguration : VScriptableObject
    {
        [Header("卡牌")] [LabelText("评分等级信息")] public Dictionary<string, VCardLevelInfo> cardLevels;

        [LabelText("卡牌容量")] public List<VCardCapacityInfo> cardCapacities;

        [Header("遗物")] [LabelText("遗物容量")] public Dictionary<string, int> relicCount;

        [LabelText("阶段性事件对应遗物")] public List<VRelicRewardInfo> relicRewards;

        [Header("属性")] [LabelText("属性效果等级容量")] public Dictionary<int, int> effectCapacities;

        [LabelText("直播属性词条")] public List<VAbilityEffectInfo> streamAttributeEffects;

        [Space(3)] [LabelText("其他属性词条")] public List<VAttributeEffectInfo> attributeEffects;

        [LabelText("属性等级信息")] public Dictionary<string, VEffectLevelInfo> attributeLevels;
    }
}