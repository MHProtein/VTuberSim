using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.Character.Attributes
{
    public class VAbilityAttribute : VCharacterAttribute
    {
        public readonly Color color;
        public VAbilityAttribute(VCharacterAttributeConfiguration configuration, Color color, int initialValue, 
            VRaisingEventKey eventKey = VRaisingEventKey.Default, int maxValue = Int32.MaxValue, 
            int minValue = 0, bool isPercentage = false)
            : base(configuration, initialValue, eventKey, maxValue, minValue, isPercentage)
        {
            this.color = color;
        }

        public void AddAbility(int delta, bool shouldMultiplyByEfficiency = false)
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
            SendEvent(Value, delta);
        }
        
        public override KeyValuePair<string, VBattleAttribute> ConvertToBattleAttribute()
        {
            float conversionRate = 0;
            if (_attributeManager.TryGetAttributeValue(AttributeName + "ConversionRatio",
                    out var value, out var isPercentage))
            {
                conversionRate = value / 100f;
            }
            
            return new KeyValuePair<string, VBattleAttribute>(_configuration.battleAttributeName,
                (VBattleAttribute)Activator.CreateInstance(BattleAttributeType, Mathf.CeilToInt(Value * conversionRate * 100f), color));
        }
    }
}