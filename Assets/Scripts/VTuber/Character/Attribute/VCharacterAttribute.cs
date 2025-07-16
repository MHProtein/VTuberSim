using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;

namespace VTuber.Character.Attribute
{
    public class VCharacterAttribute
    {
        public bool IsConvertToBattleAttribute => _configuration.isConvertToBattleAttribute;
        private VCharacterAttributeConfiguration _configuration;
        
        protected int _minValue;
        protected int _maxValue;
        protected VRaisingEventKey _eventKey;
        
        public int Value { get; private set; }
        
        public VCharacterAttribute(VCharacterAttributeConfiguration configuration, int initialValue, VRaisingEventKey eventKey = VRaisingEventKey.Default,
            int maxValue = Int32.MaxValue, int minValue = 0)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _eventKey = eventKey;
            _configuration = configuration;
            Value = initialValue;
        }
        
        public string GetAttributeName()
        {
            return _configuration.attributeName;   
        }
        
        public string GetBattleAttributeName()
        {
            return IsConvertToBattleAttribute ? _configuration.attributeName : "";
        }

        public virtual VBattleAttribute ConvertToBattleAttribute()
        {
            if (!IsConvertToBattleAttribute)
            {
                return null;
            }
            
            return new VBattleAttribute(Value, _configuration.isBattleAttributePercentage);
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
                { "Delta", delta },
            };
            VRaisingRootEventCenter.Instance.Raise(_eventKey, messageDict);
        }
        
    }
}