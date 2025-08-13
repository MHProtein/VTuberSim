using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.Character.Attributes
{
    [Serializable]
    public struct VRangeValueMap<T>
    {
        [HorizontalGroup]
        public int from;
        [HorizontalGroup]
        public int to;
        [HorizontalGroup]
        public T value;
        
        public bool IsInRange(int v)
        {
            if(to == -1)
                return v >= from;
            return v >= from && v <= to;
        }
    }
    
    public class VAbilityAttribute : VCharacterAttribute
    {
        public readonly Color color;
        private List<VRangeValueMap<float>> _abilityGainFromBattleRates;
        public VAbilityAttribute(VCharacterAttributeConfiguration configuration, 
            List<VRangeValueMap<float>> abilityGainFromBattleRates, Color color, int initialValue, 
            VRaisingEventKey eventKey = VRaisingEventKey.Default, int maxValue = Int32.MaxValue, 
            int minValue = 0, bool isPercentage = false)
            : base(configuration, initialValue, eventKey, maxValue, minValue, isPercentage)
        {
            this.color = color;
            _abilityGainFromBattleRates = abilityGainFromBattleRates;
        }

        public void AddAbility(int delta, bool shouldMultiplyByEfficiency)
        {
            if (delta == 0)
                return;

            if (shouldMultiplyByEfficiency)
            {
                float gainEfficiency = 0;
                if (_attributeManager.TryGetAttributeValue(AttributeName + "GainEfficiency",
                        out var value, out var isPercentage))
                {
                    gainEfficiency = value / 100f;
                }
                
                delta = (int)(delta * gainEfficiency);
            }

            Value = Mathf.Clamp(delta + Value, _minValue, _maxValue);
            VDebug.Log($"Added {delta} to {AttributeName}, new value: {Value}");
            SendEvent(Value, delta);
        }
        
        public override KeyValuePair<string, VBattleAttribute> ConvertToBattleAttribute()
        {
            int conversionRate = 0;
            if (_attributeManager.TryGetAttributeValue(AttributeName + "ConversionRatio",
                    out var value, out var isPercentage))
            {
                conversionRate = value;
            }
            
            return new KeyValuePair<string, VBattleAttribute>(_configuration.battleAttributeName,
                (VBattleAttribute)Activator.CreateInstance(BattleAttributeType, Value * conversionRate, color));
        }

        public override void ConvertToAttribute(Dictionary<string, VBattleAttribute> battleAttributes)
        {;
            if (battleAttributes.TryGetValue("BAPopularity", out var battleAttribute))
            {
                var popularity = battleAttribute as VBattlePopularityAttribute;
                var score = popularity.ScoreForAbilities[_configuration.battleAttributeName];

                foreach (var rateItem in _abilityGainFromBattleRates)
                {
                    int to = rateItem.to;
                    if (to == -1)
                        to = int.MaxValue;
                    
                    if(score >= rateItem.from && score <= to)
                    {
                        int abilityGain = (int)(rateItem.value * score);
                        AddAbility(abilityGain, false);
                        VDebug.Log($"Converted {score} to ability gain: {abilityGain}, now total: {Value}");
                        return;
                    }
                }
            }
            else
            {
                VDebug.LogWarning($"Battle attribute BAPopularity not found in battle attributes.");
            }
        }
    }
}