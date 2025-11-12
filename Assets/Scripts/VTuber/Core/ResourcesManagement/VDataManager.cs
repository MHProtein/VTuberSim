using System.Collections.Generic;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Consumable;
using VTuber.CoopSystem;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;
using VTuber.EventSystem.Events;
using VTuber.Relic;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Events.DialogueEvent;

namespace VTuber.Core.Managers
{
    public class VDataManager : VSingleton<VDataManager>
    {
        public Dictionary<uint, VCardConfiguration> CardConfigurations { get; private set; }

        public Dictionary<uint, VEffectConfiguration> EffectConfigurations { get; private set; }

        public Dictionary<uint, VBuffConfiguration> BuffConfigurations { get; private set; }

        public Dictionary<uint, VEffectCondition> Conditions { get; private set; }

        public Dictionary<uint, VRaisingEffectConfiguration> RaisingEffects { get; private set; }

        public Dictionary<uint, VCardCondition> CardConditions { get; private set; }

        public Dictionary<uint, VDialogueEventConfiguration> DialogueEventConfigs { get; private set; }

        public Dictionary<uint, VStreamEventConfiguration> StreamEventConfigs { get; private set; }

        public Dictionary<uint, VRaisingRelicCondition> RaisingRelicConditions { get; private set; }

        public Dictionary<uint, VRelicConfiguration> Relics { get; private set; }

        public Dictionary<uint, VCoopEvent> CoopEvents { get; private set; }

        public Dictionary<uint, VPlacingCondition> PlacingConditions { get; private set; }

        public Dictionary<uint, VSchedulingCondition> SchedulingConditions { get; private set; }

        public Dictionary<uint, VConsumableConfiguration> ConsumableConfigurationss { get; private set; }


        public void SetCardConfigurations(List<VCardConfiguration> cardConfigurations)
        {
            CardConfigurations = new Dictionary<uint, VCardConfiguration>();

            foreach (var cardConfig in cardConfigurations)
                if (cardConfig != null)
                    CardConfigurations[cardConfig.id] = cardConfig;
        }

        public void SetEffectConfigurations(List<VEffectConfiguration> effectConfigurations)
        {
            EffectConfigurations = new Dictionary<uint, VEffectConfiguration>();

            foreach (var effectConfig in effectConfigurations)
                if (effectConfig != null)
                    EffectConfigurations[effectConfig.id] = effectConfig;
        }

        public void SetBuffConfigurations(List<VBuffConfiguration> buffConfigurations)
        {
            BuffConfigurations = new Dictionary<uint, VBuffConfiguration>();

            foreach (var buffConfig in buffConfigurations)
                if (buffConfig != null)
                    BuffConfigurations[buffConfig.id] = buffConfig;
        }

        public void SetConditions(List<VEffectCondition> newConditions)
        {
            Conditions = new Dictionary<uint, VEffectCondition>();

            foreach (var condition in newConditions)
                if (condition != null)
                    Conditions[condition.id] = condition;
        }

        public void SetRaisingEffectConfigurations(List<VRaisingEffectConfiguration> effectConfigurations)
        {
            RaisingEffects = new Dictionary<uint, VRaisingEffectConfiguration>();

            foreach (var effectConfig in effectConfigurations)
                if (effectConfig != null)
                    RaisingEffects[effectConfig.id] = effectConfig;
        }

        public void SetCardConditions(List<VCardCondition> newConditions)
        {
            CardConditions = new Dictionary<uint, VCardCondition>();

            foreach (var condition in newConditions)
                if (condition != null)
                    CardConditions[condition.ID] = condition;
        }

        public void SetDialogueEventConfigurations(List<VDialogueEventConfiguration> dialogueEventConfigs)
        {
            DialogueEventConfigs = new Dictionary<uint, VDialogueEventConfiguration>();

            foreach (var eventConfig in dialogueEventConfigs)
                if (eventConfig != null)
                    DialogueEventConfigs[eventConfig.id] = eventConfig;
        }

        public void SetStreamEventConfigurations(List<VStreamEventConfiguration> streamEventConfigs)
        {
            StreamEventConfigs = new Dictionary<uint, VStreamEventConfiguration>();

            foreach (var eventConfig in streamEventConfigs)
                if (eventConfig != null)
                    StreamEventConfigs[eventConfig.id] = eventConfig;
        }

        public void SetRelicConditions(List<VRaisingRelicCondition> relicConditions)
        {
            RaisingRelicConditions = new Dictionary<uint, VRaisingRelicCondition>();

            foreach (var condition in relicConditions)
                if (condition != null)
                    RaisingRelicConditions[condition.Id] = condition;
        }

        public void SetRelics(List<VRelicConfiguration> relics)
        {
            Relics = new Dictionary<uint, VRelicConfiguration>();

            foreach (var relic in relics)
                if (relic != null)
                    Relics[relic.id] = relic;
        }

        public void SetCoopEvents(List<VCoopEvent> coopEvents)
        {
            CoopEvents = new Dictionary<uint, VCoopEvent>();

            foreach (var coopEvent in coopEvents)
                if (coopEvent != null)
                    CoopEvents[coopEvent.id] = coopEvent;
        }

        public void SetPlacingConditon(List<VPlacingCondition> conditions)
        {
            PlacingConditions = new Dictionary<uint, VPlacingCondition>();

            foreach (var condition in conditions)
                if (condition != null)
                    PlacingConditions[condition.Id] = condition;
        }

        public void SetConsumableConfigurations(List<VConsumableConfiguration> consumableConfigurations)
        {
            ConsumableConfigurationss = new Dictionary<uint, VConsumableConfiguration>();

            foreach (var consumableConfig in consumableConfigurations)
                if (consumableConfig != null)
                    ConsumableConfigurationss[consumableConfig.id] = consumableConfig;
        }

        public void SetSchedulingConditions(List<VSchedulingCondition> conditions)
        {
            SchedulingConditions = new Dictionary<uint, VSchedulingCondition>();
            foreach (var condition in conditions)
                if (condition != null)
                    SchedulingConditions[condition.Id] = condition;
        }

        public VSchedulingCondition GetSchedulingConditionByID(uint id)
        {
            return SchedulingConditions[id];
        }

        public VRelic CreateRelicByID(uint id)
        {
            if (Relics.TryGetValue(id, out var relicConfiguration)) return relicConfiguration.CreateRelic();

            return null;
        }

        public VEffect CreateEffectByID(uint effectID, string parameter, string upgradedParameter)
        {
            if (EffectConfigurations.TryGetValue(effectID, out var effectConfig))
                return effectConfig.CreateEffect(parameter, upgradedParameter);

            return null;
        }

        public VCard CreateCardByID(uint cardID)
        {
            if (CardConfigurations.TryGetValue(cardID, out var cardConfig)) return cardConfig.CreateCard();

            return null;
        }

        public VBuff CreateBuffByID(uint buffID)
        {
            if (BuffConfigurations.TryGetValue(buffID, out var buffConfig)) return buffConfig.CreateBuff();

            return null;
        }

        public VRaisingEffect CreateRaisingEffectByID(uint effectID, string parameter, string upgradedParameter)
        {
            if (RaisingEffects.TryGetValue(effectID, out var effectConfig))
                return effectConfig.CreateEffect(parameter, upgradedParameter);
            return null;
        }

        public VStreamEvent CreateStreamEventByID(uint eventID)
        {
            if (StreamEventConfigs.TryGetValue(eventID, out var eventConfig))
                return eventConfig.CreateEvent() as VStreamEvent;
            return null;
        }

        public VDialogueEvent CreateDialogueEventByID(uint eventID)
        {
            if (DialogueEventConfigs.TryGetValue(eventID, out var eventConfig))
                return eventConfig.CreateEvent() as VDialogueEvent;
            return null;
        }

        public List<VCardConfiguration> GetAllCardConfigurations()
        {
            return new List<VCardConfiguration>(CardConfigurations.Values);
        }

        public List<VConsumableConfiguration> GetAllConsumableConfigurations()
        {
            return new List<VConsumableConfiguration>(ConsumableConfigurationss.Values);
        }

        public List<VScheduleEventConfiguration> GetAllEventConfigurations()
        {
            var allEvents = new List<VScheduleEventConfiguration>();
            allEvents.AddRange(DialogueEventConfigs.Values);
            allEvents.AddRange(StreamEventConfigs.Values);
            return allEvents;
        }

        public VEffectCondition GetConditionByID(uint conditionID)
        {
            return Conditions.GetValueOrDefault(conditionID);
        }

        public VEffectConfiguration GetEffectConfigurationByID(uint effectID)
        {
            return EffectConfigurations.GetValueOrDefault(effectID);
        }

        public VCardConfiguration GetCardConfigurationByID(uint cardID)
        {
            return CardConfigurations.GetValueOrDefault(cardID);
        }

        public VBuffConfiguration GetBuffConfigurationByID(uint buffID)
        {
            return BuffConfigurations.GetValueOrDefault(buffID);
        }

        public VCardCondition GetCardConditionByID(uint conditionID)
        {
            return CardConditions.GetValueOrDefault(conditionID);
        }

        public VDialogueEventConfiguration GetDialogueEventConfigurationByID(uint eventID)
        {
            return DialogueEventConfigs.GetValueOrDefault(eventID);
        }

        public VStreamEventConfiguration GetStreamEventConfigurationByID(uint eventID)
        {
            return StreamEventConfigs.GetValueOrDefault(eventID);
        }

        public VRaisingRelicCondition GetRaisingRelicCondition(uint conditionID)
        {
            return RaisingRelicConditions.GetValueOrDefault(conditionID);
        }

        public VCoopEvent GetCoopEventByID(uint eventID)
        {
            return CoopEvents.GetValueOrDefault(eventID);
        }

        public VScheduleEvent CreateEvent(VEventType eventType, uint id)
        {
            if (eventType == VEventType.Stream) return CreateStreamEventByID(id);

            return CreateDialogueEventByID(id);
        }

        public VPlacingCondition GetPlacingCondtionByID(uint conditionId)
        {
            return PlacingConditions.GetValueOrDefault(conditionId);
        }

        public VConsumable CreateConsumableByID(uint consumableID)
        {
            if (ConsumableConfigurationss.TryGetValue(consumableID, out var consumableConfig))
                return consumableConfig.CreateConsumable();
            return null;
        }
    }
}