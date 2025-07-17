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
        protected VCharacterAttributeConfiguration _configuration;
        
        protected int _minValue;
        protected int _maxValue;
        protected VRaisingEventKey _eventKey;
        
        public VValueModifier<float> GainRateModifier => gainRateModifier;
        protected VValueModifier<float> gainRateModifier;
        
        public VValueModifier<int> GainPointsModifier => gainPointsModifier;
        protected VValueModifier<int> gainPointsModifier;
        
        public int Value { get; protected set; }
        
        protected VCharacterAttributeManager _attributeManager;
        
        public VCharacterAttribute(VCharacterAttributeConfiguration configuration, 
            int initialValue, VRaisingEventKey eventKey = VRaisingEventKey.Default,
            int maxValue = Int32.MaxValue, int minValue = 0, bool isPercentage = false)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _eventKey = eventKey;
            InitSetValue(initialValue);
        }

        public void SetAttributeManager(VCharacterAttributeManager attributeManager)
        {
            _attributeManager = attributeManager;
        }
        
        public string GetAttributeName()
        {
            return _configuration.attributeName;   
        }
        
        public string GetBattleAttributeName()
        {
            return isConvertToBattleAttribute ? _configuration.attributeName : "";
        }

        public virtual VBattleAttribute ConvertToBattleAttribute()
        {
            if (!isConvertToBattleAttribute)
            {
                return null;
            }
            
            return new VBattleAttribute(Value, isBattleAttributePercentage);
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
        
        protected int GetModifierIntValue(VValueModifier<int> modifier)
        {
            if (modifier.Modifiers.Count == 0)
                return modifier.DefaultValue;
            int total = modifier.DefaultValue;
            foreach (var mod in modifier.Modifiers)
            {
                total += mod.Value;
            }
            return total;
        }
        
        protected float GetModifierFloatValue(VValueModifier<float> modifier)
        {
            if (modifier.Modifiers.Count == 0)
                return modifier.DefaultValue;
            float total = modifier.DefaultValue;
            foreach (var mod in modifier.Modifiers)
            {
                total += mod.Value;
            }
            return total;
        }
    }
}