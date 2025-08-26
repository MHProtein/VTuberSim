using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.Character.Attribute
{
    public class VCharacterAttribute
    {
        protected VCharacterAttributeConfiguration _configuration;
        
        
        public string AttributeName { get; set; }
        
        public int Value { get; protected set; }
        
        protected VCharacterAttributeManager _attributeManager;
        public bool IsConvertToBattleAttribute => _configuration.isConvertToBattleAttribute;
        public bool IsBattleAttributePercentage => _configuration.isBattleAttributePercentage;
        public VBattleEventKey BattleEventKey => _configuration.battleEventKey;
        public bool IsPercentage { get; protected set; }
        public bool IsBattleAttribute => _configuration.isConvertToBattleAttribute && 
                                         _configuration.battleAttribute != null;
        public Type BattleAttributeType => _configuration.battleAttribute.TypeToSerialize;

        public bool ShouldBattleAttributeConvertTo { get; private set; }

        public int MaxValue => _maxValue;
        protected int _minValue;
        protected int _maxValue;
        protected VRaisingEventKey _eventKey;
        
        public VValueModifier<float> GainRateModifier => gainRateModifier;
        protected VValueModifier<float> gainRateModifier;
        
        public VValueModifier<int> GainPointsModifier => gainPointsModifier;
        protected VValueModifier<int> gainPointsModifier;
        
        public VCharacterAttribute(VCharacterAttributeConfiguration configuration, 
            int initialValue, VRaisingEventKey eventKey = VRaisingEventKey.Default,
            int maxValue = Int32.MaxValue, int minValue = 0, bool isPercentage = false, bool shouldBattleAttributeConvertTo = true)
        {
            _configuration = configuration;
            _minValue = minValue;
            _maxValue = maxValue;
            _eventKey = eventKey;
            IsPercentage = isPercentage;
            SetValue(initialValue);
            ShouldBattleAttributeConvertTo = shouldBattleAttributeConvertTo;
            gainPointsModifier = new VValueModifier<int>(0);
            gainRateModifier = new VValueModifier<float>(1.0f);
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
            return IsConvertToBattleAttribute ? _configuration.battleAttributeName : "";
        }

        public virtual KeyValuePair<string, VBattleAttribute> ConvertToBattleAttribute()
        {
            if (!IsConvertToBattleAttribute)
            {
                return new KeyValuePair<string, VBattleAttribute>("", null);
            }
            
            return new KeyValuePair<string, VBattleAttribute>(_configuration.battleAttributeName,
                (VBattleAttribute)Activator.CreateInstance(BattleAttributeType,
                    Value,
                    _configuration.isBattleAttributePercentage,
                    _configuration.battleEventKey,
                    _maxValue, _minValue));
        }

        public virtual void ConvertToAttribute(Dictionary<string, VBattleAttribute> battleAttributes)
        {
            if (!ShouldBattleAttributeConvertTo)
                return;

            try
            {
                var battleAttribute = battleAttributes[_configuration.battleAttributeNameWhenConvertBack];
                
                SetValue(battleAttribute.Value);
            
                VDebug.Log("Converted to attribute: " + _configuration.battleAttributeNameWhenConvertBack + ", value: " + Value + 
                           ", battle attribute value: " + battleAttribute.Value);
            }
            catch (Exception e)
            {
                VDebug.LogError(e);
            }
            
        }
        
        public virtual void AddTo(int delta)
        {
            if (delta == 0)
                return;
            int temp = Value;
            int gainPointsModifierValue = VValueModifier<int>.GetModifierIntValue(gainPointsModifier);
            float gainRateModifierValue = VValueModifier<float>.GetModifierFloatValue(gainRateModifier);
            int finalDelta = (int)((delta + gainPointsModifierValue) * (gainRateModifierValue));
            if(delta < 0 && finalDelta > 0)
                finalDelta = 0;
            Value = Mathf.Clamp(Value + finalDelta,
                _minValue, _maxValue);
            VDebug.Log("添加 (变化量:" + delta + " + " + gainPointsModifierValue + ") * " + gainRateModifierValue + " = " + finalDelta
                       + " 到 " + AttributeName + "，新数值: " + Value);
            SendEvent(Value, Value - temp);
        }
        
        public int PreviewAddTo(int delta)
        {
            if (delta == 0)
                return Value;
            int gainPointsModifierValue = VValueModifier<int>.GetModifierIntValue(gainPointsModifier);
            float gainRateModifierValue = VValueModifier<float>.GetModifierFloatValue(gainRateModifier);
            int finalDelta = (int)((delta + gainPointsModifierValue) * (gainRateModifierValue));
            if(delta < 0 && finalDelta > 0)
                finalDelta = 0;
            return Value + finalDelta;
        }

        
        public virtual void MultiplyWith(int delta)
        {         
            if (delta == 1)
                return;
            int temp = Value;
            Value = Mathf.Clamp(Value * delta, _minValue, _maxValue);
            SendEvent(Value, Value - temp);
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
                { "MaxValue", _maxValue },
                { "Delta", delta },
            };
            VRaisingRootEventCenter.Instance.Raise(_eventKey, messageDict);
        }

        public void AddMaxValue(int value)
        {
            _maxValue += value;
            SendEvent(Value, 0);
            VDebug.Log("Added max value: " + value + " to " + _configuration.attributeName + ", new max value: " + _maxValue);
        }
    }
}