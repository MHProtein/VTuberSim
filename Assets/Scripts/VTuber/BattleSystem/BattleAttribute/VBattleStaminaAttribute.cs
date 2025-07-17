using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleStaminaAttribute : VBattleAttribute
    {
        public VBattleStaminaAttribute(int value, int maxValue = Int32.MaxValue, int minValue = 0)
            : base(value, false, VBattleEventKey.OnStaminaChange, maxValue, minValue)
        {
            gainRateModifier = new VValueModifier<float>(0.0f);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
        }

        private void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            //ResetConsumeModifiers();
        }
        
        public override void AddTo(int delta, bool isFromCard, bool shouldApplyTwice = false)
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
                SendEvent(Value, delta, isFromCard);
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
    }
}