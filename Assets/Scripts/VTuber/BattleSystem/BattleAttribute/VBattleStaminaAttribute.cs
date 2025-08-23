using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleStaminaAttribute : VBattleAttribute
    {
        public VBattleStaminaAttribute(int value, VBattleEventKey key, bool isShield = false, int maxValue = Int32.MaxValue, int minValue = 0)
            : base(value, false, key, maxValue, minValue)
        {
            if (isShield)
            {
                gainPointsModifier.SetEventKey(VBattleEventKey.OnShieldModifierChanged);
                gainRateModifier.SetEventKey(VBattleEventKey.OnShieldModifierChanged);
            }
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
            SetValue(Mathf.Clamp(delta + Value, _minValue, _maxValue), isFromCard, shouldApplyTwice);
            VDebug.Log($"{AttributeName} 消耗: {delta}, 当前数值: {Value})");
        }
        


        public bool TestCost(int cost)
        {
            if (cost == 0)
                return true;

            return Value + cost >= 0;
        }
    }
}