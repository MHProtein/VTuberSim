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
    public class VResourcesManager : VSingleton<VResourcesManager>
    {
        public Dictionary<uint, VCardConfiguration> CardConfigurations => _cardConfigurations;
        private Dictionary<uint, VCardConfiguration> _cardConfigurations;

        public Dictionary<uint, VEffectConfiguration> EffectConfigurations => _effectConfigurations;
        private Dictionary<uint, VEffectConfiguration> _effectConfigurations;

        public Dictionary<uint, VBuffConfiguration> BuffConfigurations => _buffConfigurations;
        private Dictionary<uint, VBuffConfiguration> _buffConfigurations;

        public Dictionary<uint, VEffectCondition> Conditions => _conditions;
        private Dictionary<uint, VEffectCondition> _conditions;
        
        public Dictionary<uint, VRaisingEffectConfiguration> RaisingEffects => _raisingEffects;
        private Dictionary<uint, VRaisingEffectConfiguration> _raisingEffects;
        
        public Dictionary<uint, VCardCondition> CardConditions => _cardConditions;
        private Dictionary<uint, VCardCondition> _cardConditions;
        
        public Dictionary<uint, VDialogueEventConfiguration> DialogueEventConfigs => _dialogueEventConfigs;
        private Dictionary<uint, VDialogueEventConfiguration> _dialogueEventConfigs;
        
        public Dictionary<uint, VStreamEventConfiguration> StreamEventConfigs => _streamEventConfigs;
        private Dictionary<uint, VStreamEventConfiguration> _streamEventConfigs;
        
        public Dictionary<uint, VRaisingRelicCondition> RaisingRelicConditions => _raisingRelicConditions;
        private Dictionary<uint, VRaisingRelicCondition> _raisingRelicConditions;
        
        public Dictionary<uint, VRelicConfiguration> Relics => _relics;
        private Dictionary<uint, VRelicConfiguration> _relics;
        
        public Dictionary<uint, VCoopEvent> CoopEvents => _coopEvents;
        private Dictionary<uint, VCoopEvent> _coopEvents;
        
        public Dictionary<uint, VPlacingCondition> PlacingConditions => _placingConditions;
        private Dictionary<uint, VPlacingCondition> _placingConditions;
        
        public Dictionary<uint, VConsumableConfiguration> ConsumableConfigurationss => _consumableConfigurations;
        private Dictionary<uint, VConsumableConfiguration> _consumableConfigurations;
        
        public void SetCardConfigurations(List<VCardConfiguration> cardConfigurations)
        {
            _cardConfigurations = new Dictionary<uint, VCardConfiguration>();

            foreach (var cardConfig in cardConfigurations)
            {
                if (cardConfig != null)
                {
                    _cardConfigurations[cardConfig.id] = cardConfig;
                }
            }

        }

        public void SetEffectConfigurations(List<VEffectConfiguration> effectConfigurations)
        {
            _effectConfigurations = new Dictionary<uint, VEffectConfiguration>();

            foreach (var effectConfig in effectConfigurations)
            {
                if (effectConfig != null)
                {
                    _effectConfigurations[effectConfig.id] = effectConfig;
                }
            }
        }

        public void SetBuffConfigurations(List<VBuffConfiguration> buffConfigurations)
        {
            _buffConfigurations = new Dictionary<uint, VBuffConfiguration>();

            foreach (var buffConfig in buffConfigurations)
            {
                if (buffConfig != null)
                {
                    _buffConfigurations[buffConfig.id] = buffConfig;
                }
            }
        }

        public void SetConditions(List<VEffectCondition> newConditions)
        {
            _conditions = new Dictionary<uint, VEffectCondition>();

            foreach (var condition in newConditions)
            {
                if (condition != null)
                {
                    _conditions[condition.id] = condition;
                }
            }
        }
        
        public void SetRaisingEffectConfigurations(List<VRaisingEffectConfiguration> effectConfigurations)
        {
            _raisingEffects = new Dictionary<uint, VRaisingEffectConfiguration>();

            foreach (var effectConfig in effectConfigurations)
            {
                if (effectConfig != null)
                {
                    _raisingEffects[effectConfig.id] = effectConfig;
                }
            }
        }
        
        public void SetCardConditions(List<VCardCondition> newConditions)
        {
            _cardConditions = new Dictionary<uint, VCardCondition>();

            foreach (var condition in newConditions)
            {
                if (condition != null)
                {
                    _cardConditions[condition.ID] = condition;
                }
            }
        }
        
        public void SetDialogueEventConfigurations(List<VDialogueEventConfiguration> dialogueEventConfigs)
        {
            _dialogueEventConfigs = new Dictionary<uint, VDialogueEventConfiguration>();

            foreach (var eventConfig in dialogueEventConfigs)
            {
                if (eventConfig != null)
                {
                    _dialogueEventConfigs[eventConfig.id] = eventConfig;
                }
            }
        }
        
        public void SetStreamEventConfigurations(List<VStreamEventConfiguration> streamEventConfigs)
        {
            _streamEventConfigs = new Dictionary<uint, VStreamEventConfiguration>();

            foreach (var eventConfig in streamEventConfigs)
            {
                if (eventConfig != null)
                {
                    _streamEventConfigs[eventConfig.id] = eventConfig;
                }
            }
        }
        
        public void SetRelicConditions(List<VRaisingRelicCondition> relicConditions)
        {
            _raisingRelicConditions = new Dictionary<uint, VRaisingRelicCondition>();

            foreach (var condition in relicConditions)
            {
                if (condition != null)
                {
                    _raisingRelicConditions[condition.Id] = condition;
                }
            }
        }

        public void SetRelics(List<VRelicConfiguration> relics)
        {
            _relics = new Dictionary<uint, VRelicConfiguration>();

            foreach (var relic in relics)
            {
                if (relic != null)
                {
                    _relics[relic.id] = relic;
                }
            }
        }

        public void SetCoopEvents(List<VCoopEvent> coopEvents)
        {
            _coopEvents = new Dictionary<uint, VCoopEvent>();

            foreach (var coopEvent in coopEvents)
            {
                if (coopEvent != null)
                {
                    _coopEvents[coopEvent.id] = coopEvent;
                }
            }
        }

        public void SetPlacingConditon(List<VPlacingCondition> conditions)
        {
            _placingConditions = new Dictionary<uint, VPlacingCondition>();

            foreach (var condition in conditions)
            {
                if (condition != null)
                {
                    _placingConditions[condition.Id] = condition;
                }
            }
        }

        public void SetConsumableConfigurations(List<VConsumableConfiguration> consumableConfigurations)
        {
            _consumableConfigurations = new Dictionary<uint, VConsumableConfiguration>();

            foreach (var consumableConfig in consumableConfigurations)
            {
                if (consumableConfig != null)
                {
                    _consumableConfigurations[consumableConfig.id] = consumableConfig;
                }
            }
        }

        public VRelic CreateRelicByID(uint id)
        {
            if (_relics.TryGetValue(id, out var relicConfiguration))
            {
                return relicConfiguration.CreateRelic();
            }

            return null;
        }

        public VEffect CreateEffectByID(uint effectID, string parameter, string upgradedParameter)
        {
            if (_effectConfigurations.TryGetValue(effectID, out var effectConfig))
            {
                return effectConfig.CreateEffect(parameter, upgradedParameter);
            }

            return null;
        }

        public VCard CreateCardByID(uint cardID)
        {
            if (_cardConfigurations.TryGetValue(cardID, out var cardConfig))
            {
                return cardConfig.CreateCard();
            }

            return null;
        }

        public VBuff CreateBuffByID(uint buffID)
        {
            if (_buffConfigurations.TryGetValue(buffID, out var buffConfig))
            {
                return buffConfig.CreateBuff();
            }

            return null;
        }

        public VRaisingEffect CreateRaisingEffectByID(uint effectID, string parameter, string upgradedParameter)
        {
            if (_raisingEffects.TryGetValue(effectID, out var effectConfig))
            {
                return effectConfig.CreateEffect(parameter, upgradedParameter);
            }
            return null;
        }
        
        public VStreamEvent CreateStreamEventByID(uint eventID)
        {
            if (_streamEventConfigs.TryGetValue(eventID, out var eventConfig))
            {
                return eventConfig.CreateEvent() as VStreamEvent;
            }
            return null;
        }
        
        public VDialogueEvent CreateDialogueEventByID(uint eventID)
        {
            if (_dialogueEventConfigs.TryGetValue(eventID, out var eventConfig))
            {
                return eventConfig.CreateEvent() as VDialogueEvent;
            }
            return null;
        }
        
        public List<VCardConfiguration> GetAllCardConfigurations()
        {
            return new List<VCardConfiguration>(_cardConfigurations.Values);
        }
        
        public List<VConsumableConfiguration> GetAllConsumableConfigurations()
        {
            return new List<VConsumableConfiguration>(_consumableConfigurations.Values);
        }
        
        public List<VScheduleEventConfiguration> GetAllEventConfigurations()
        {
            var allEvents = new List<VScheduleEventConfiguration>();
            allEvents.AddRange(_dialogueEventConfigs.Values);
            allEvents.AddRange(_streamEventConfigs.Values);
            return allEvents;
        }
        
        public VEffectCondition GetConditionByID(uint conditionID)
        {
            return _conditions.GetValueOrDefault(conditionID);
        }
        
        public VEffectConfiguration GetEffectConfigurationByID(uint effectID)
        {
            return _effectConfigurations.GetValueOrDefault(effectID);
        }
        
        public VCardConfiguration GetCardConfigurationByID(uint cardID)
        {
            return _cardConfigurations.GetValueOrDefault(cardID);
        }

        public VBuffConfiguration GetBuffConfigurationByID(uint buffID)
        {
            return _buffConfigurations.GetValueOrDefault(buffID);
        }

        public VCardCondition GetCardConditionByID(uint conditionID)
        {
            return _cardConditions.GetValueOrDefault(conditionID);
        }
        
        public VDialogueEventConfiguration GetDialogueEventConfigurationByID(uint eventID)
        {
            return _dialogueEventConfigs.GetValueOrDefault(eventID);
        }
        
        public VStreamEventConfiguration GetStreamEventConfigurationByID(uint eventID)
        {
            return _streamEventConfigs.GetValueOrDefault(eventID);
        }
        
        public VRaisingRelicCondition GetRaisingRelicCondition(uint conditionID)
        {
            return _raisingRelicConditions.GetValueOrDefault(conditionID);
        }
        
        public VCoopEvent GetCoopEventByID(uint eventID)
        {
            return _coopEvents.GetValueOrDefault(eventID);
        }
        
        public VScheduleEvent CreateEvent(VEventType eventType, uint id)
        {
            if (eventType == VEventType.Stream)
            {
                return CreateStreamEventByID(id);
            }

            return CreateDialogueEventByID(id);
        }

        public VPlacingCondition GetPlacingCondtionByID(uint conditionId)
        {
            return _placingConditions.GetValueOrDefault(conditionId);
        }
        
        public VConsumable CreateConsumableByID(uint consumableID)
        {
            if (_consumableConfigurations.TryGetValue(consumableID, out var consumableConfig))
            {
                return consumableConfig.CreateConsumable();
            }
            return null;
        }
    }
}