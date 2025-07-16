using System.Collections.Generic;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.Foundation;

namespace VTuber.Core.Managers
{
    public class VBattleDataManager : VSingleton<VBattleDataManager>
    {
        public Dictionary<uint, VCardConfiguration> CardConfigurations => _cardConfigurations;
        private Dictionary<uint, VCardConfiguration> _cardConfigurations;

        public Dictionary<uint, VEffectConfiguration> EffectConfigurations => _effectConfigurations;
        private Dictionary<uint, VEffectConfiguration> _effectConfigurations;

        public Dictionary<uint, VBuffConfiguration> BuffConfigurations => _buffConfigurations;
        private Dictionary<uint, VBuffConfiguration> _buffConfigurations;

        public Dictionary<uint, VEffectCondition> Conditions => conditions;
        private Dictionary<uint, VEffectCondition> conditions;

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
            conditions = new Dictionary<uint, VEffectCondition>();

            foreach (var condition in newConditions)
            {
                if (condition != null)
                {
                    conditions[condition.id] = condition;
                }
            }
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

        public VEffectCondition GetConditionByID(uint conditionID)
        {
            return conditions.GetValueOrDefault(conditionID);
        }
    }
}