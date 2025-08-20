using System;
using System.Collections.Generic;
using VTuber.Character.Attribute;
using VTuber.Character.Attributes;
using VTuber.Consumable;
using VTuber.CoopSystem;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Relic;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;

namespace VTuber.Character
{
    public class VCharacterRelicManager
    {
        public VRaisingRelicManager RaisingRelicManager => _raisingRelicManager;
        private readonly VRaisingRelicManager _raisingRelicManager;
        
        private List<VBattleRelic> _battleRelics;

        public VCharacterRelicManager(VCharacter character)
        {
            _battleRelics = new List<VBattleRelic>();
            _raisingRelicManager = new VRaisingRelicManager(character);
        }

        public List<VBattleRelic> GetBattleRelics()
        {
            return _battleRelics;
        }

        public void AddRelic(VRelic relic)
        {
            if (relic is VBattleRelic battleRelic)
            {
                _battleRelics.Add(battleRelic);
            }
            else
            {
                _raisingRelicManager.AddRelic(relic as VRaisingRelic);
            }
            VDebug.Log("Added Relic " + relic.GetRelicName());
            
        }
    }
    
    public class VCharacter
    {
        public string Name { get; set; }

        public string LiveType => _characterConfig.liveType;
        
        public uint FillingEventIDDuration1 => _characterConfig.fillingEventIDDuration1;
        public uint FillingEventIDDuration2 => _characterConfig.fillingEventIDDuration2;
        public uint FillingEventIDDuration3 => _characterConfig.fillingEventIDDuration3;
        
        public VCharacterAttributeManager AttributeManager { get; private set; }

        private VCharacterConfiguration _characterConfig;

        public VCardLibrary CardLibrary => _cardLibrary;
        private VCardLibrary _cardLibrary;

        public VCharacterRelicManager CharacterRelicManager => _characterRelicManager;
        private VCharacterRelicManager _characterRelicManager;
        
        public VCooperatorManager CooperatorManager => _cooperatorManager;
        private VCooperatorManager _cooperatorManager;
        
        public VConsumableManager ConsumableManager => _consumableManager;
        private VConsumableManager _consumableManager;
        
        public List<VScheduleEvent> eventsCompleted;
        public List<VScheduleEvent> succeededStreams;
        
        public VCharacter(VCharacterConfiguration characterConfig)
        {
            _cardLibrary = new VCardLibrary();
            _cooperatorManager = new VCooperatorManager();
            _consumableManager = new VConsumableManager(this);
            InitializeAttributes(characterConfig);
            _characterRelicManager = new VCharacterRelicManager(this);
            eventsCompleted = new List<VScheduleEvent>();
        }
        
        public void OnEnable()
        {
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventBeginExecute, OnEventExecuted);
        }

        public void OnDisable()
        {
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventBeginExecute, OnEventExecuted);
        }
        
        void InitializeAttributes(VCharacterConfiguration characterConfig)
        {
            _characterConfig = characterConfig;
            
            AttributeManager = new VCharacterAttributeManager();
            AttributeManager.AddAttribute("CAStamina",
                new VStaminaAttribute(characterConfig.staminaConfiguration, 
                    characterConfig.staminaInitialValue, 
                    VRaisingEventKey.OnStaminaChanged, 
                    characterConfig.staminaMaxValue == -1 ? int.MaxValue : characterConfig.staminaMaxValue,
                    characterConfig.staminaMinValue));
            
            AttributeManager.AddAttribute("CAPressure",
                new VPressureAttribute(characterConfig.pressureConfiguration, 
                    characterConfig.pressureBuffs,
                    characterConfig.pressureInitialValue, 
                    VRaisingEventKey.OnPressureChanged, 
                    characterConfig.pressureMaxValue == -1 ? int.MaxValue : characterConfig.pressureMaxValue,
                    characterConfig.pressureMinValue));
            
            AttributeManager.AddAttribute("CASingingAbility", 
                new VAbilityAttribute(characterConfig.singingAbilityConfiguration,  
                    characterConfig.singingAbilityGainFromBattleRates,    
                    characterConfig.singingAbilityColor,
                    characterConfig.singingAbilityInitialValue, 
                    VRaisingEventKey.OnSingingAbilityChanged, 
                    characterConfig.singingAbilityMaxValue == -1 ? int.MaxValue : characterConfig.singingAbilityMaxValue,
                    characterConfig.singingAbilityMinValue));

            AttributeManager.AddAttribute("CAGamingAbility",
                new VAbilityAttribute(characterConfig.gamingAbilityConfiguration,
                    characterConfig.gamingAbilityGainFromBattleRates,
                    characterConfig.gamingAbilityColor,
                    characterConfig.gamingAbilityInitialValue, 
                    VRaisingEventKey.OnGamingAbilityChanged, 
                    characterConfig.gamingAbilityMaxValue == -1 ? int.MaxValue : characterConfig.gamingAbilityMaxValue,
                    characterConfig.gamingAbilityMinValue));
            
            AttributeManager.AddAttribute("CAChattingAbility",
                new VAbilityAttribute(characterConfig.chattingAbilityConfiguration,
                    characterConfig.chattingAbilityGainFromBattleRates,
                    characterConfig.chattingAbilityColor,
                    characterConfig.chattingAbilityInitialValue, 
                    VRaisingEventKey.OnChattingAbilityChanged, 
                    characterConfig.chattingAbilityMaxValue == -1 ? int.MaxValue : characterConfig.chattingAbilityMaxValue,
                    characterConfig.chattingAbilityMinValue));
            
            AttributeManager.AddAttribute("CASingingAbilityConversionRatio",
                new VConversionRatioAttribute(characterConfig.singingAbilityConversionRatioConfiguration,
                    characterConfig.singingAbilityConversionRatioInitialValue,
                    VRaisingEventKey.OnSingingAbilityConversionRatioChanged,
                    characterConfig.singingAbilityConversionRatioMaxValue == -1 ? int.MaxValue : characterConfig.singingAbilityConversionRatioMaxValue,
                    characterConfig.singingAbilityConversionRatioMinValue)
                );
            
            AttributeManager.AddAttribute("CAGamingAbilityConversionRatio",
                new VConversionRatioAttribute(characterConfig.gamingAbilityConversionRatioConfiguration,
                    characterConfig.gamingAbilityConversionRatioInitialValue,
                    VRaisingEventKey.OnGamingAbilityConversionRatioChanged,
                    characterConfig.gamingAbilityConversionRatioMaxValue == -1 ? int.MaxValue : characterConfig.gamingAbilityConversionRatioMaxValue,
                    characterConfig.gamingAbilityConversionRatioMinValue));
                    
            AttributeManager.AddAttribute("CAChattingAbilityConversionRatio",
                new VConversionRatioAttribute(characterConfig.chattingAbilityConversionRatioConfiguration,
                    characterConfig.chattingAbilityConversionRatioInitialValue,
                    VRaisingEventKey.OnChattingAbilityConversionRatioChanged,
                    characterConfig.chattingAbilityConversionRatioMaxValue == -1 ? int.MaxValue : characterConfig.chattingAbilityConversionRatioMaxValue,
                    characterConfig.chattingAbilityConversionRatioMinValue));
            
            AttributeManager.AddAttribute("CASingingAbilityGainEfficiency",
                new VAbilityGainEfficiencyAttribute(characterConfig.singingAbilityGainEfficiencyConfiguration,
                    characterConfig.singingAbilityGainEfficiencyInitialValue,
                    VRaisingEventKey.OnSingingAbilityGainEfficiencyChanged,
                    characterConfig.singingAbilityGainEfficiencyMaxValue == -1 ? int.MaxValue : characterConfig.singingAbilityGainEfficiencyMaxValue,
                    characterConfig.singingAbilityGainEfficiencyMinValue));
            
            AttributeManager.AddAttribute("CAGamingAbilityGainEfficiency",
                new VAbilityGainEfficiencyAttribute(characterConfig.gamingAbilityGainEfficiencyConfiguration,
                    characterConfig.gamingAbilityGainEfficiencyInitialValue,
                    VRaisingEventKey.OnGamingAbilityGainEfficiencyChanged,
                    characterConfig.gamingAbilityGainEfficiencyMaxValue == -1 ? int.MaxValue : characterConfig.gamingAbilityGainEfficiencyMaxValue,
                    characterConfig.gamingAbilityGainEfficiencyMinValue));
            
            AttributeManager.AddAttribute("CAChattingAbilityGainEfficiency",
                new VAbilityGainEfficiencyAttribute(characterConfig.chattingAbilityGainEfficiencyConfiguration,
                    characterConfig.chattingAbilityGainEfficiencyInitialValue,
                    VRaisingEventKey.OnChattingAbilityGainEfficiencyChanged,
                    characterConfig.chattingAbilityGainEfficiencyMaxValue == -1 ? int.MaxValue : characterConfig.chattingAbilityGainEfficiencyMaxValue,
                    characterConfig.chattingAbilityGainEfficiencyMinValue));
            
            AttributeManager.AddAttribute("CAFollowerCount", 
                new VFollowerCountAttribute(characterConfig.followerCountConfiguration,
                    characterConfig.followerCountInitialValue, 
                    VRaisingEventKey.OnFollowerCountChanged, 
                    characterConfig.followerCountMaxValue == -1 ? int.MaxValue : characterConfig.followerCountMaxValue,
                    characterConfig.followerCountMinValue));
            
            AttributeManager.AddAttribute("CAMembershipCount",
                new VMembershipCountAttribute(characterConfig.membershipCountConfiguration,
                    characterConfig.membershipCountInitialValue, characterConfig.membershipBuffs,
                    VRaisingEventKey.OnMemberCountChanged, 
                    characterConfig.membershipCountMaxValue == -1 ? int.MaxValue : characterConfig.membershipCountMaxValue,
                    characterConfig.membershipCountMinValue));
            
            AttributeManager.AddAttribute("CAFollowerToViewerRatio",
                new VConversionRatioAttribute(characterConfig.followerToViewerRatioConfiguration,
                    characterConfig.followerToViewerRatioInitialValue, 
                    VRaisingEventKey.OnFollowerToViewerRatioChanged, 
                    characterConfig.followerToViewerRatioMaxValue == -1 ? int.MaxValue : characterConfig.followerToViewerRatioMaxValue,
                    characterConfig.followerToViewerRatioMinValue));
            
            AttributeManager.AddAttribute("CAMoney",
                new VMoneyAttribute(characterConfig.moneyConfiguration,
                    characterConfig.moneyInitialValue, 
                    VRaisingEventKey.OnMoneyChanged, 
                    characterConfig.moneyMaxValue == -1 ? int.MaxValue : characterConfig.moneyMaxValue,
                    characterConfig.moneyMinValue));
            
            AttributeManager.AddAttribute("CARevenueShareRate",
                new VCharacterAttribute(characterConfig.revenueShareRateConfiguration,
                    characterConfig.revenueShareRateInitialValue, 
                    VRaisingEventKey.OnRevenueShareRateChanged, 
                    characterConfig.revenueShareRateMaxValue == -1 ? int.MaxValue : characterConfig.revenueShareRateMaxValue,
                    characterConfig.revenueShareRateMinValue, true));
            
            AttributeManager.AddAttribute("CASkipEventStaminaRecovery",
                new VCharacterAttribute(characterConfig.skipEventStaminaRecoveryConfiguration,
                    characterConfig.skipEventStaminaRecoveryInitialValue, 
                    VRaisingEventKey.OnSkipEventStaminaChanged, 
                    characterConfig.skipEventStaminaRecoveryMaxValue == -1 ? int.MaxValue : characterConfig.skipEventStaminaRecoveryMaxValue,
                    characterConfig.skipEventStaminaRecoveryMinValue, true));
            
            AttributeManager.AddAttribute("CASkipTurnStaminaRecovery",
                new VCharacterAttribute(characterConfig.skipTurnStaminaRecoveryConfiguration,
                    characterConfig.skipTurnStaminaRecoveryInitialValue, 
                    VRaisingEventKey.OnSkipTurnStaminaChanged, 
                    characterConfig.skipTurnStaminaRecoveryMaxValue == -1 ? int.MaxValue : characterConfig.skipTurnStaminaRecoveryMaxValue,
                    characterConfig.skipTurnStaminaRecoveryMinValue, true));
        }
        
        public bool TestCost(VScheduleEvent e)
        {
            return AttributeManager.TestCost(e);
        }

        public void ApplyCost(VScheduleEvent e)
        {
            AttributeManager.ApplyCost(e);
        }
        
        private void OnEventExecuted(Dictionary<string, object> messagedict)
        {
            var e = messagedict["Event"] as VScheduleEvent;
            eventsCompleted.Add(e);
        }
        
        public bool HasCompletedEvent(VEventType type, uint eventID)
        {
            foreach (var e in eventsCompleted)
            {
                if (e.Type == type && e.EventID == eventID)
                {
                    return true;
                }
            }
            return false;
        }

        public void SkipEventRecoverStamina()
        {
            AttributeManager.TryGetAttribute("CAStamina", out var stamina);
            AttributeManager.TryGetAttribute("CASkipTurnStaminaRecovery", out var recoveryAmount);
            stamina.AddTo(recoveryAmount.Value);
        }
    }
}













