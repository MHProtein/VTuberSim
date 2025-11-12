using System;
using System.Collections.Generic;
using System.Linq;
using SlayTheSpire.System.SavingSystem;
using VTuber.BattleSystem.Card;
using VTuber.Character.Attribute;
using VTuber.Character.Attributes;
using VTuber.Consumable;
using VTuber.CoopSystem;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;

namespace VTuber.Character
{
    public class VCharacterSaveData
    {
        public List<VCharacterAttributeSaveData> attributes;
        public List<VCardSaveData> cardSaveDatas;
        public string characterConfigurationName;
        public List<uint> consumables;
        public List<VCoopSaveData> cooperatorSaveData;
        public Dictionary<VEventType, List<uint>> eventsCompleted;
        public List<uint> relicIds;
        public List<uint> succeededStreams;
    }

    public class VCharacter
    {
        private VCharacterConfiguration _characterConfig;

        public Dictionary<VEventType, List<uint>> eventsCompleted;
        public List<uint> succeededStreams;

        public VCharacter(VCharacterConfiguration characterConfig)
        {
            if (characterConfig is null)
                return;
            InitializeAttributes(characterConfig);
        }

        public string Name => _characterConfig.characterName;

        public string LiveType => _characterConfig.liveType;

        public uint FillingEventIDDuration1 => _characterConfig.fillingEventIDDuration1;
        public uint FillingEventIDDuration2 => _characterConfig.fillingEventIDDuration2;
        public uint FillingEventIDDuration3 => _characterConfig.fillingEventIDDuration3;

        public VCharacterAttributeManager AttributeManager { get; private set; }

        public VCardLibrary CardLibrary { get; private set; }

        public VCharacterRelicManager CharacterRelicManager { get; private set; }

        public VCooperatorManager CooperatorManager { get; private set; }

        public VConsumableManager ConsumableManager { get; private set; }

        public void Initialize(bool isLoaded)
        {
            CardLibrary = new VCardLibrary();
            CooperatorManager = new VCooperatorManager();
            ConsumableManager = new VConsumableManager(this);
            CharacterRelicManager = new VCharacterRelicManager(this);
            eventsCompleted = new Dictionary<VEventType, List<uint>>();

            foreach (var eventType in Enum.GetValues(typeof(VEventType)))
                eventsCompleted.Add((VEventType)eventType, new List<uint>());

            succeededStreams = new List<uint>();

            if (isLoaded)
                return;

            CardLibrary.AddCard(VDataManager.Instance.CreateCardByID(_characterConfig.initialCardId));
            CharacterRelicManager.AddRelic(VDataManager.Instance.CreateRelicByID(_characterConfig.initialRelicId));
        }

        public void OnEnable()
        {
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventBeginExecute, OnEventExecuted);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnDayEnd, OnDayEnd);
        }

        public void OnDisable()
        {
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventBeginExecute, OnEventExecuted);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnDayEnd, OnDayEnd);
        }

        private void OnDayEnd(Dictionary<string, object> messagedict)
        {
            AttributeManager.ApplyPressureEffects(this);
        }

        private void InitializeAttributes(VCharacterConfiguration characterConfig)
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
                    characterConfig.pressureEffects,
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
                    characterConfig.singingAbilityMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.singingAbilityMaxValue,
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
                    characterConfig.chattingAbilityMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.chattingAbilityMaxValue,
                    characterConfig.chattingAbilityMinValue));

            AttributeManager.AddAttribute("CASingingAbilityConversionRatio",
                new VConversionRatioAttribute(characterConfig.singingAbilityConversionRatioConfiguration,
                    characterConfig.singingAbilityConversionRatioInitialValue,
                    VRaisingEventKey.OnSingingAbilityConversionRatioChanged,
                    characterConfig.singingAbilityConversionRatioMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.singingAbilityConversionRatioMaxValue,
                    characterConfig.singingAbilityConversionRatioMinValue)
            );

            AttributeManager.AddAttribute("CAGamingAbilityConversionRatio",
                new VConversionRatioAttribute(characterConfig.gamingAbilityConversionRatioConfiguration,
                    characterConfig.gamingAbilityConversionRatioInitialValue,
                    VRaisingEventKey.OnGamingAbilityConversionRatioChanged,
                    characterConfig.gamingAbilityConversionRatioMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.gamingAbilityConversionRatioMaxValue,
                    characterConfig.gamingAbilityConversionRatioMinValue));

            AttributeManager.AddAttribute("CAChattingAbilityConversionRatio",
                new VConversionRatioAttribute(characterConfig.chattingAbilityConversionRatioConfiguration,
                    characterConfig.chattingAbilityConversionRatioInitialValue,
                    VRaisingEventKey.OnChattingAbilityConversionRatioChanged,
                    characterConfig.chattingAbilityConversionRatioMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.chattingAbilityConversionRatioMaxValue,
                    characterConfig.chattingAbilityConversionRatioMinValue));

            AttributeManager.AddAttribute("CASingingAbilityGainEfficiency",
                new VAbilityGainEfficiencyAttribute(characterConfig.singingAbilityGainEfficiencyConfiguration,
                    characterConfig.singingAbilityGainEfficiencyInitialValue,
                    VRaisingEventKey.OnSingingAbilityGainEfficiencyChanged,
                    characterConfig.singingAbilityGainEfficiencyMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.singingAbilityGainEfficiencyMaxValue,
                    characterConfig.singingAbilityGainEfficiencyMinValue));

            AttributeManager.AddAttribute("CAGamingAbilityGainEfficiency",
                new VAbilityGainEfficiencyAttribute(characterConfig.gamingAbilityGainEfficiencyConfiguration,
                    characterConfig.gamingAbilityGainEfficiencyInitialValue,
                    VRaisingEventKey.OnGamingAbilityGainEfficiencyChanged,
                    characterConfig.gamingAbilityGainEfficiencyMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.gamingAbilityGainEfficiencyMaxValue,
                    characterConfig.gamingAbilityGainEfficiencyMinValue));

            AttributeManager.AddAttribute("CAChattingAbilityGainEfficiency",
                new VAbilityGainEfficiencyAttribute(characterConfig.chattingAbilityGainEfficiencyConfiguration,
                    characterConfig.chattingAbilityGainEfficiencyInitialValue,
                    VRaisingEventKey.OnChattingAbilityGainEfficiencyChanged,
                    characterConfig.chattingAbilityGainEfficiencyMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.chattingAbilityGainEfficiencyMaxValue,
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
                    characterConfig.membershipCountMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.membershipCountMaxValue,
                    characterConfig.membershipCountMinValue));

            AttributeManager.AddAttribute("CAFollowerToViewerRatio",
                new VConversionRatioAttribute(characterConfig.followerToViewerRatioConfiguration,
                    characterConfig.followerToViewerRatioInitialValue,
                    VRaisingEventKey.OnFollowerToViewerRatioChanged,
                    characterConfig.followerToViewerRatioMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.followerToViewerRatioMaxValue,
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
                    characterConfig.revenueShareRateMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.revenueShareRateMaxValue,
                    characterConfig.revenueShareRateMinValue, true));

            AttributeManager.AddAttribute("CASkipEventStaminaRecovery",
                new VCharacterAttribute(characterConfig.skipEventStaminaRecoveryConfiguration,
                    characterConfig.skipEventStaminaRecoveryInitialValue,
                    VRaisingEventKey.OnSkipEventStaminaChanged,
                    characterConfig.skipEventStaminaRecoveryMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.skipEventStaminaRecoveryMaxValue,
                    characterConfig.skipEventStaminaRecoveryMinValue, false, false));

            AttributeManager.AddAttribute("CASkipTurnStaminaRecovery",
                new VCharacterAttribute(characterConfig.skipTurnStaminaRecoveryConfiguration,
                    characterConfig.skipTurnStaminaRecoveryInitialValue,
                    VRaisingEventKey.OnSkipTurnStaminaChanged,
                    characterConfig.skipTurnStaminaRecoveryMaxValue == -1
                        ? int.MaxValue
                        : characterConfig.skipTurnStaminaRecoveryMaxValue,
                    characterConfig.skipTurnStaminaRecoveryMinValue));
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
            eventsCompleted[e.Type].Add(e.EventID);
        }

        public bool HasCompletedEvent(VEventType type, uint eventID)
        {
            return eventsCompleted[type].Contains(eventID);
        }

        public void SkipEventRecoverStamina()
        {
            AttributeManager.TryGetAttribute("CAStamina", out var stamina);
            AttributeManager.TryGetAttribute("CASkipTurnStaminaRecovery", out var recoveryAmount);
            stamina.AddTo(recoveryAmount.Value, true);
        }

        public void EndRun()
        {
            ConsumableManager.Clear();
            CharacterRelicManager.Clear();
            CardLibrary.Clear();
        }

        public void Load(SaveData data, VCharacterConfiguration characterConfiguration)
        {
            var characterSaveData = data.characterSaveData;
            _characterConfig = characterConfiguration;
            InitializeAttributes(_characterConfig);

            foreach (var attributeSaveData in characterSaveData.attributes)
            {
                AttributeManager.TryGetAttribute(attributeSaveData.attributeName, out var attribute);
                attribute.Load(attributeSaveData);
            }

            Initialize(true);
            foreach (var cardSave in characterSaveData.cardSaveDatas)
            {
                var card = VDataManager.Instance.CreateCardByID(cardSave.configID);
                card.Load(cardSave);
                CardLibrary.AddCard(card);
            }

            foreach (var relicId in characterSaveData.relicIds)
                CharacterRelicManager.AddRelic(VDataManager.Instance.CreateRelicByID(relicId));

            foreach (var consumableId in characterSaveData.consumables)
                ConsumableManager.AddConsumable(VDataManager.Instance.CreateConsumableByID(consumableId));

            foreach (var cooperatorSaveData in characterSaveData.cooperatorSaveData)
                CooperatorManager.AddCooperator(VCooperator.Load(cooperatorSaveData));

            eventsCompleted = characterSaveData.eventsCompleted;
            succeededStreams = characterSaveData.succeededStreams;
        }

        public void Save(SaveData data)
        {
            var characterSaveData = new VCharacterSaveData
            {
                characterConfigurationName = _characterConfig.name,
                cardSaveDatas = CardLibrary.GetCards().Select(card => card.Save()).ToList(),
                attributes = AttributeManager.GetAttributes().Select(attribute => attribute.Save()).ToList(),
                relicIds = CharacterRelicManager.GetRelics().Select(relic => relic.ConfigId).ToList(),
                consumables = ConsumableManager.GetConsumables().Select(consumable => consumable.ConfigId).ToList(),
                cooperatorSaveData =
                    CooperatorManager.GetCooperators().Select(cooperator => cooperator.Save()).ToList(),
                eventsCompleted = eventsCompleted,
                succeededStreams = succeededStreams
            };
            data.characterSaveData = characterSaveData;
        }
    }
}