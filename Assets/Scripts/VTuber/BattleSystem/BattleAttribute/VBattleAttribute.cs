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
            InitSetValue(value);
            _isPercentage = isPercentage;
        }
        
        public virtual void AddTo(int delta)
        {
            if (delta == 0)
                return;
            int temp = Value;
            Value = Mathf.Clamp(Value + delta, _minValue, _maxValue);
            SendEvent(Value, Value - temp);
        }
        
        public virtual void MultiplyWith(int delta)
        {         
            if (delta == 1)
                return;
            int temp = Value;
            Value = Mathf.Clamp(Value * delta, _minValue, _maxValue);
            SendEvent(Value, Value - temp);
        }

        private void InitSetValue(int value)
        {
            Value = Mathf.Clamp(value, _minValue, _maxValue);
            SendEvent(Value, value - Value);
        }
        
        protected virtual void SetValue(int value)
        {
            Value = Mathf.Clamp(value, _minValue, _maxValue);
            SendEvent(Value, value - Value);
        }
        
        public void SendEvent(int newValue, int delta)  
        {
            var messageDict = new Dictionary<string, object>
            {
                { "NewValue", newValue },
                { "Delta", delta }
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