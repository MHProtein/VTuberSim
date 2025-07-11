using System;
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

        public void ChangeConsumeRate(float rate)
        {
            consumeRate += rate;
        }
        
        public void ChangeConsumeReducedPoints(int points)
        {
            consumeReducedPoints += points;
        }
        
        public override void AddTo(int delta)
        {
            if (delta == 0)
            {
                ResetConsumeModifiers();
                return;
            }
            if (delta > 0)
            {
                Value = Mathf.Clamp(delta + Value, _minValue, _maxValue);
                ResetConsumeModifiers();
            }
            else
            {
                delta = (int)(delta * consumeRate) + consumeReducedPoints;
                
                if (delta > 0)
                    delta = 0;
                
                Value = Mathf.Clamp(delta + Value, _minValue, _maxValue);
                VDebug.Log($"Stamina consumed: {delta}, current value: {Value}, consume rate: {consumeRate}, reduced points: {consumeReducedPoints}");
                ResetConsumeModifiers();
            }
            
            if (delta != 0)
                SendEvent(Value, delta);
        }
        
        private void ResetConsumeModifiers()
        {
            consumeRate = 1f;
            consumeReducedPoints = 0;
        }
    }
}