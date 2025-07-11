using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using VTuber.BattleSystem.Buff;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.BattleAttribute
{
    
    //All the attributes treated as int type, if is percentage, it is multiplied by 100 and vice versa when used. 
    public class VBattleAttribute
    {
        public int Value { get; protected set; }
        protected int _minValue;
        protected int _maxValue;
        private bool _isPercentage;
        protected VRootEventKey _eventKey;
        
        public VBattleAttribute(int value, bool isPercentage = false, VRootEventKey eventKey = VRootEventKey.Default, int maxValue = Int32.MaxValue, int minValue = 0)
        {
            _eventKey = eventKey;
            _minValue = minValue;
            _maxValue = maxValue;
            InitSetValue(value, false);
            _isPercentage = isPercentage;
        }
        
        public virtual void AddTo(int delta, bool isFromCard, bool shouldPlayTwice = false)
        {
            if (delta == 0)
                return;
            int temp = Value;
            Value = Mathf.Clamp(Value + delta, _minValue, _maxValue);
            SendEvent(Value, Value - temp, isFromCard, shouldPlayTwice);
        }
        
        public virtual void MultiplyWith(int delta, bool isFromCard, bool shouldPlayTwice = false)
        {         
            if (delta == 1)
                return;
            int temp = Value;
            Value = Mathf.Clamp(Value * delta, _minValue, _maxValue);
            SendEvent(Value, Value - temp, isFromCard, shouldPlayTwice);
        }

        private void InitSetValue(int value, bool isFromCard, bool shouldPlayTwice = false)
        {
            Value = Mathf.Clamp(value, _minValue, _maxValue);
            SendEvent(Value, value - Value, isFromCard, shouldPlayTwice);
        }
        
        protected virtual void SetValue(int value, bool isFromCard, bool shouldPlayTwice = false)
        {
            Value = Mathf.Clamp(value, _minValue, _maxValue);
            SendEvent(Value, value - Value, isFromCard, shouldPlayTwice);
        }
        
        public void SendEvent(int newValue, int delta, bool isFromCard, bool shouldPlayTwice = false)  
        {
            var messageDict = new Dictionary<string, object>
            {
                { "NewValue", newValue },
                { "Delta", delta },
                {"IsFromCard", isFromCard },
                {"ShouldPlayTwice", shouldPlayTwice }
            };
            VBattleRootEventCenter.Instance.Raise(_eventKey, messageDict);
        }

        public virtual void OnEnable()
        {
            
        }
        
        public virtual void OnDisable()
        {
            
        }
        
    }
}