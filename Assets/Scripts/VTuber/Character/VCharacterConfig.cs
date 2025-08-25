using System.Collections.Generic;
using System.Linq;
using VTuber.Character.Attribute;
using VTuber.Core.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.Character.Attributes;

namespace VTuber.Character
{
    public class VCharacterConfiguration : VScriptableObject
    {
        public string characterName;
        [TextArea] public string description;
        public string liveType;
        public uint initialCardId;
        public uint initialRelicId;
        #region AttributeConfigs
        
        [HorizontalGroup("StaminaGroup", Gap = 10)]
        [Header("体力")]
        [LabelText("初始值")]
        public int staminaInitialValue;

        [HorizontalGroup("StaminaGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int staminaMinValue;

        [HorizontalGroup("StaminaGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int staminaMaxValue;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration staminaConfiguration;

        // Pressure
        [HorizontalGroup("PressureGroup", Gap = 10)]
        [Header("压力")]
        [LabelText("初始值")]
        public int pressureInitialValue;

        [HorizontalGroup("PressureGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int pressureMinValue;

        [HorizontalGroup("PressureGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int pressureMaxValue;

        [LabelText("压力Buff表")]
        [DictionaryDrawerSettings(KeyLabel = "BuffID", ValueLabel = "Layer",
            DisplayMode = DictionaryDisplayOptions.OneLine)]
        public List<int> pressureBuffs = new List<int>();
        
        [LabelText("压力效果表")]
        [DictionaryDrawerSettings(KeyLabel = "效果ID", ValueLabel = "参数",
            DisplayMode = DictionaryDisplayOptions.OneLine)]
        public Dictionary<uint, string> pressureEffects = new ();
        
        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration pressureConfiguration;
        
        // Singing Ability
        [HorizontalGroup("SingingGroup", Gap = 10)]
        [Header("歌力")]
        [LabelText("初始值")]
        public int singingAbilityInitialValue;

        [HorizontalGroup("SingingGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int singingAbilityMinValue;

        [HorizontalGroup("SingingGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int singingAbilityMaxValue;
        
        [LabelText("歌力颜色")][ColorPalette] public Color singingAbilityColor;
        
        public List<VRangeValueMap<float>> singingAbilityGainFromBattleRates;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration singingAbilityConfiguration;

        // Gaming Ability
        [HorizontalGroup("GamingGroup", Gap = 10)]
        [Header("游戏力")]
        [LabelText("初始值")]
        public int gamingAbilityInitialValue;

        [HorizontalGroup("GamingGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int gamingAbilityMinValue;

        [HorizontalGroup("GamingGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int gamingAbilityMaxValue;
        
        [LabelText("游戏力颜色")][ColorPalette] public Color gamingAbilityColor;

        public List<VRangeValueMap<float>> gamingAbilityGainFromBattleRates;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration gamingAbilityConfiguration;

        // Chatting Ability
        [HorizontalGroup("ChattingGroup", Gap = 10)]
        [Header("杂谈力")]
        [LabelText("初始值")]
        public int chattingAbilityInitialValue;

        [HorizontalGroup("ChattingGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int chattingAbilityMinValue;

        [HorizontalGroup("ChattingGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int chattingAbilityMaxValue;
        
        [LabelText("杂谈力颜色")][ColorPalette] public Color chattingAbilityColor;
        public List<VRangeValueMap<float>> chattingAbilityGainFromBattleRates;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration chattingAbilityConfiguration;

        // Singing Ability Conversion Ratio
        [HorizontalGroup("SingingRatioGroup", Gap = 10)]
        [Header("歌力转化率%")]
        [LabelText("初始值")]
        public int singingAbilityConversionRatioInitialValue;

        [HorizontalGroup("SingingRatioGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int singingAbilityConversionRatioMinValue;

        [HorizontalGroup("SingingRatioGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int singingAbilityConversionRatioMaxValue;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration singingAbilityConversionRatioConfiguration;

        // Gaming Ability Conversion Ratio
        [HorizontalGroup("GamingRatioGroup", Gap = 10)]
        [Header("游戏力转化率%")]
        [LabelText("初始值")]
        public int gamingAbilityConversionRatioInitialValue;

        [HorizontalGroup("GamingRatioGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int gamingAbilityConversionRatioMinValue;

        [HorizontalGroup("GamingRatioGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int gamingAbilityConversionRatioMaxValue;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration gamingAbilityConversionRatioConfiguration;

        // Chatting Ability Conversion Ratio
        [HorizontalGroup("ChattingRatioGroup", Gap = 10)]
        [Header("杂谈力转化率%")]
        [LabelText("初始值")]
        public int chattingAbilityConversionRatioInitialValue;

        [HorizontalGroup("ChattingRatioGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int chattingAbilityConversionRatioMinValue;

        [HorizontalGroup("ChattingRatioGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int chattingAbilityConversionRatioMaxValue;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration chattingAbilityConversionRatioConfiguration;

        // Singing Gain Efficiency
        [HorizontalGroup("SingingEfficiencyGroup", Gap = 10)]
        [Header("歌力获得效率%")]
        [LabelText("初始值")]
        public int singingAbilityGainEfficiencyInitialValue;

        [HorizontalGroup("SingingEfficiencyGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int singingAbilityGainEfficiencyMinValue;

        [HorizontalGroup("SingingEfficiencyGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int singingAbilityGainEfficiencyMaxValue;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration singingAbilityGainEfficiencyConfiguration;

        // Gaming Gain Efficiency
        [HorizontalGroup("GamingEfficiencyGroup", Gap = 10)]
        [Header("游戏力获得效率%")]
        [LabelText("初始值")]
        public int gamingAbilityGainEfficiencyInitialValue;

        [HorizontalGroup("GamingEfficiencyGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int gamingAbilityGainEfficiencyMinValue;

        [HorizontalGroup("GamingEfficiencyGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int gamingAbilityGainEfficiencyMaxValue;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration gamingAbilityGainEfficiencyConfiguration;

        // Chatting Gain Efficiency
        [HorizontalGroup("ChattingEfficiencyGroup", Gap = 10)]
        [Header("杂谈力获得效率%")]
        [LabelText("初始值")]
        public int chattingAbilityGainEfficiencyInitialValue;

        [HorizontalGroup("ChattingEfficiencyGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int chattingAbilityGainEfficiencyMinValue;

        [HorizontalGroup("ChattingEfficiencyGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int chattingAbilityGainEfficiencyMaxValue;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration chattingAbilityGainEfficiencyConfiguration;

        // Follower Count
        [HorizontalGroup("FollowerGroup", Gap = 10)]
        [Header("粉丝数")]
        [LabelText("初始值")]
        public int followerCountInitialValue;

        [HorizontalGroup("FollowerGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int followerCountMinValue;

        [HorizontalGroup("FollowerGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int followerCountMaxValue;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration followerCountConfiguration;

        // Membership Count
        [HorizontalGroup("MembershipGroup", Gap = 10)]
        [Header("舰长数")]
        [LabelText("初始值")]
        public int membershipCountInitialValue;

        [HorizontalGroup("MembershipGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int membershipCountMinValue;

        [HorizontalGroup("MembershipGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int membershipCountMaxValue;
        
        [LabelText("舰长Buff表")]
        public List<VRangeValueMap<uint>> membershipBuffs = new List<VRangeValueMap<uint>>();

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration membershipCountConfiguration;

        // Follower-to-Viewer Ratio
        [HorizontalGroup("FollowerRatioGroup", Gap = 10)]
        [Header("粉丝同接转化率%")]
        [LabelText("初始值")]
        public int followerToViewerRatioInitialValue;

        [HorizontalGroup("FollowerRatioGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int followerToViewerRatioMinValue;

        [HorizontalGroup("FollowerRatioGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int followerToViewerRatioMaxValue;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration followerToViewerRatioConfiguration;

        // Money
        [HorizontalGroup("MoneyGroup", Gap = 10)]
        [Header("金钱")]
        [LabelText("初始值")]
        public int moneyInitialValue;

        [HorizontalGroup("MoneyGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int moneyMinValue;

        [HorizontalGroup("MoneyGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int moneyMaxValue;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration moneyConfiguration;
        
        // Money
        [HorizontalGroup("RevenueShareRateGroup", Gap = 10)]
        [Header("直播分成率%")]
        [LabelText("初始值")]
        public int revenueShareRateInitialValue;

        [HorizontalGroup("RevenueShareRateGroup")]
        [Header("")]
        [LabelText("最小值")]
        public int revenueShareRateMinValue;

        [HorizontalGroup("RevenueShareRateGroup")]
        [Header("")]
        [LabelText("最大值")]
        public int revenueShareRateMaxValue;

        [PropertySpace(10)]
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration revenueShareRateConfiguration;
        
        [HorizontalGroup("SkipEventStaminaRecovery", Gap = 10)]
        [Header("跳过事件回复体力")]
        [LabelText("初始值")]
        public int skipEventStaminaRecoveryInitialValue;

        [HorizontalGroup("SkipEventStaminaRecovery")]
        [Header("")]
        [LabelText("最小值")]
        public int skipEventStaminaRecoveryMinValue;

        [HorizontalGroup("SkipEventStaminaRecovery")]
        [Header("")]
        [LabelText("最大值")]
        public int skipEventStaminaRecoveryMaxValue;
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration skipEventStaminaRecoveryConfiguration;
        
        [HorizontalGroup("SkipTurnStaminaRecovery", Gap = 10)]
        [Header("跳过回合回复体力")]
        [LabelText("初始值")]
        public int skipTurnStaminaRecoveryInitialValue;

        [HorizontalGroup("SkipTurnStaminaRecovery")]
        [Header("")]
        [LabelText("最小值")]
        public int skipTurnStaminaRecoveryMinValue;

        [HorizontalGroup("SkipTurnStaminaRecovery")]
        [Header("")]
        [LabelText("最大值")]
        public int skipTurnStaminaRecoveryMaxValue;
        [LabelText("Config文件")]
        public VCharacterAttributeConfiguration skipTurnStaminaRecoveryConfiguration;
        #endregion
        
        [FormerlySerializedAs("fillingEventIDSize1")]
        [Header("补充事件")]
        [SerializeField] [LabelText("补充事件(大小1)")] public uint fillingEventIDDuration1 = 0;
        [FormerlySerializedAs("fillingEventIDSize2")] [SerializeField] [LabelText("补充事件(大小2)")] public uint fillingEventIDDuration2 = 0;
        [FormerlySerializedAs("fillingEventIDSize3")] [SerializeField] [LabelText("补充事件(大小3)")] public uint fillingEventIDDuration3 = 0;
    }
}