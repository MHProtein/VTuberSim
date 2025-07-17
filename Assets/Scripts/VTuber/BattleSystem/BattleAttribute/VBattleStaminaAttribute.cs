using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleStaminaAttribute : VBattleAttribute
    {
        public VValueModifier<float> ConsumeRateModifier => consumeRateModifier;
        protected VValueModifier<float> consumeRateModifier;
        
        public VValueModifier<int> ConsumePointsModifier => consumePointsModifier;
        protected VValueModifier<int> consumePointsModifier;
        
        public VBattleStaminaAttribute(int value, VBattleEventKey key, int maxValue = Int32.MaxValue, int minValue = 0)
            : base(value, false, key, maxValue, minValue)
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
                base.AddTo(delta, isFromCard, shouldApplyTwice);
                return;
            }
            
            Value = Mathf.Clamp(CalculateCost(ref delta, Value), _minValue, _maxValue);
            VDebug.Log($"Stamina consumed: {delta}, current value: {Value}, " +
                       $"reduced rate: {GetModifierFloatValue(consumeRateModifier)}, reduced points: {GetModifierIntValue(consumePointsModifier)}");
            
            if (delta != 0)
                SendEvent(Value, delta, isFromCard);
        }
        
        public int CalculateCost(ref int delta, int value)
        {
            delta = (int)(delta * (1.0f - GetModifierFloatValue(consumeRateModifier))) + GetModifierIntValue(consumePointsModifier);
            
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