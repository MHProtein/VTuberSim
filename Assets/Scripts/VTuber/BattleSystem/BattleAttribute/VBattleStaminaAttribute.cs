using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleStaminaAttribute : VBattleAttribute
    {
        private float consumeRate = 1f;
        private int consumeReducedPoints;
        
        
        
        public VBattleStaminaAttribute(int value, int maxValue = Int32.MaxValue, int minValue = 0)
            : base(value, false, VRootEventKey.OnStaminaChange, maxValue, minValue)
        {
            
        }

        public override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
        }

        private void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            ResetConsumeModifiers();
        }
        
        public void ChangeConsumeRate(float rate)
        {
            consumeRate = rate;
        }
        
        public void ChangeConsumeReducedPoints(int points)
        {
            consumeReducedPoints = points;
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
                VDebug.Log($"Stamina consumed: {delta}, current value: {Value}, consume rate: {consumeRate}, reduced points: {consumeReducedPoints}");
            }
            
            if (delta != 0)
                SendEvent(Value, delta, isFromCard);
        }
        
        public int CalculateCost(ref int delta, int value)
        {
            delta = (int)(delta * consumeRate) + consumeReducedPoints;
                
            if (delta > 0)
                delta = 0;
                
            return delta + value;
        }
        
        private void ResetConsumeModifiers()
        {
            consumeRate = 1f;
            consumeReducedPoints = 0;
        }

        public bool TestCost(int cost)
        {
            if(cost == 0)
                return true;

            return CalculateCost(ref cost, Value) >= 0;
        }
    }
}