using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.Character.Attributes
{
    public class VStaminaAttribute : VCharacterAttribute
    {
        public VStaminaAttribute(VCharacterAttributeConfiguration configuration, int initialValue, VRaisingEventKey eventKey,
            int maxValue, int minValue) : base(configuration, initialValue, eventKey, maxValue, minValue)
        {
            gainRateModifier = new VValueModifier<float>(0.0f);
        }
        
        public override void AddTo(int delta)
        {
            if (delta == 0)
                return;
            
            if (delta > 0)
            {
                Value = Mathf.Clamp(delta + Value, _minValue, _maxValue);
            }
            else
            {
                Value = Mathf.Clamp(CalculateCost(ref delta, Value), _minValue, _maxValue);
                VDebug.Log($"Stamina consumed: {delta}, current value: {Value}, " +
                           $"reduced rate: {GetModifierFloatValue(gainRateModifier)}, reduced points: {GetModifierIntValue(GainPointsModifier)}");
            }
            
            if (delta != 0)
                SendEvent(Value, delta);
        }
        
        public int CalculateCost(ref int delta, int value)
        {
            delta = (int)(delta * (1.0f - GetModifierFloatValue(gainRateModifier))) + GetModifierIntValue(GainPointsModifier);
            
            if (delta > 0)
                delta = 0;
            
            return delta + value;
        }
        
        // private void ResetConsumeModifiers()
        // {
        //     consumeRate = 1f;
        //     consumeReducedPoints = 0;
        // }

        public bool TestCost(int cost)
        {
            if (cost == 0)
                return true;

            return CalculateCost(ref cost, Value) >= 0;
            
        }

        public override KeyValuePair<string, VBattleAttribute> ConvertToBattleAttribute()
        {
            return new KeyValuePair<string, VBattleAttribute>(_configuration.battleAttributeName,
                new VBattleStaminaAttribute(Value, VBattleEventKey.OnStaminaChange, _configuration.maxValue, _configuration.minValue));
        }

    }
}