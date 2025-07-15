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
        public Dictionary<int, VCardConfiguration> CardConfigurations => _cardConfigurations;
        private Dictionary<int, VCardConfiguration> _cardConfigurations;

        public Dictionary<int, VEffectConfiguration> EffectConfigurations => _effectConfigurations;
        private Dictionary<int, VEffectConfiguration> _effectConfigurations;

        public Dictionary<int, VBuffConfiguration> BuffConfigurations => _buffConfigurations;
        private Dictionary<int, VBuffConfiguration> _buffConfigurations;

        public Dictionary<int, VEffectCondition> Conditions => conditions;
        private Dictionary<int, VEffectCondition> conditions;

        public void SetCardConfigurations(List<VCardConfiguration> cardConfigurations)
        {
            _cardConfigurations = new Dictionary<int, VCardConfiguration>();

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
            _effectConfigurations = new Dictionary<int, VEffectConfiguration>();

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
            _buffConfigurations = new Dictionary<int, VBuffConfiguration>();

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
            conditions = new Dictionary<int, VEffectCondition>();

            foreach (var condition in newConditions)
            {
                if (condition != null)
                {
                    conditions[condition.id] = condition;
                }
            }
        }

        public VEffect CreateEffectByID(int effectID, string parameter, string upgradedParameter)
        {
            if (_effectConfigurations.TryGetValue(effectID, out var effectConfig))
            {
                return effectConfig.CreateEffect(parameter, upgradedParameter);
            }

            return null;
        }

        public VCard CreateCardByID(int cardID)
        {
            if (_cardConfigurations.TryGetValue(cardID, out var cardConfig))
            {
                return cardConfig.CreateCard();
            }

            return null;
        }

        public VBuff CreateBuffByID(int buffID)
        {
            if (_buffConfigurations.TryGetValue(buffID, out var buffConfig))
            {
                return buffConfig.CreateBuff();
            }

            return null;
        }

        public VEffectCondition GetConditionByID(int conditionID)
        {
            return conditions.GetValueOrDefault(conditionID);
        }
    }
}