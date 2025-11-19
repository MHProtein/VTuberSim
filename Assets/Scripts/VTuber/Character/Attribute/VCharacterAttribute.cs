using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.Character.Attribute
{
    public class VCharacterAttributeSaveData
    {
        public string attributeName;
        public VValueModifier<int> gainPointsModifier;
        public VValueModifier<float> gainRateModifier;
        public int value;
    }

    public class VCharacterAttribute
    {
        protected VCharacterAttributeManager _attributeManager;
        protected VCharacterAttributeConfiguration _configuration;
        protected VRaisingEventKey _eventKey;
        protected int _maxValue;
        protected int _minValue;
        protected VValueModifier<int> gainPointsModifier;
        protected VValueModifier<float> gainRateModifier;

        public VCharacterAttribute(VCharacterAttributeConfiguration configuration,
            int initialValue, VRaisingEventKey eventKey = VRaisingEventKey.Default,
            int maxValue = int.MaxValue, int minValue = 0, bool isPercentage = false,
            bool shouldBattleAttributeConvertTo = true, bool shouldSetValue = true)
        {
            _configuration = configuration;
            _minValue = minValue;
            _maxValue = maxValue;
            _eventKey = eventKey;
            IsPercentage = isPercentage;
            if(shouldSetValue) SetValue(initialValue, false);
            ShouldBattleAttributeConvertTo = shouldBattleAttributeConvertTo;
            gainPointsModifier = new VValueModifier<int>(0);
            gainRateModifier = new VValueModifier<float>(1.0f);
        }

        public string AttributeName { get; set; }

        public int Value { get; protected set; }
        public bool IsConvertToBattleAttribute => _configuration.isConvertToBattleAttribute;
        public bool IsBattleAttributePercentage => _configuration.isBattleAttributePercentage;
        public VBattleEventKey BattleEventKey => _configuration.battleEventKey;
        public bool IsPercentage { get; protected set; }

        public bool IsBattleAttribute => _configuration.isConvertToBattleAttribute &&
                                         _configuration.battleAttribute != null;

        public Type BattleAttributeType => _configuration.battleAttribute.TypeToSerialize;

        public bool ShouldBattleAttributeConvertTo { get; }

        public int MaxValue => _maxValue;

        public VValueModifier<float> GainRateModifier => gainRateModifier;

        public VValueModifier<int> GainPointsModifier => gainPointsModifier;

        public void Load(VCharacterAttributeSaveData saveData)
        {
            SetValue(saveData.value, false);
            gainRateModifier = saveData.gainRateModifier;
            gainPointsModifier = saveData.gainPointsModifier;
        }

        public VCharacterAttributeSaveData Save()
        {
            return new VCharacterAttributeSaveData
            {
                attributeName = AttributeName,
                value = Value,
                gainRateModifier = gainRateModifier,
                gainPointsModifier = gainPointsModifier
            };
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
            if (!IsConvertToBattleAttribute) return new KeyValuePair<string, VBattleAttribute>("", null);

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

                SetValue(battleAttribute.Value, false);

                VDebug.Log("Converted to attribute: " + _configuration.battleAttributeNameWhenConvertBack +
                           ", value: " + Value +
                           ", battle attribute value: " + battleAttribute.Value);
            }
            catch (Exception e)
            {
                VDebug.LogError(e);
            }
        }

        public virtual void AddTo(int delta, bool shouldPlaySFX)
        {
            if (delta == 0)
                return;
            var temp = Value;
            var gainPointsModifierValue = VValueModifier<int>.GetModifierIntValue(gainPointsModifier, true);
            var gainRateModifierValue = VValueModifier<float>.GetModifierFloatValue(gainRateModifier, true);
            var finalDelta = (int)((delta + gainPointsModifierValue) * gainRateModifierValue);
            if (delta < 0 && finalDelta > 0)
                finalDelta = 0;
            Value = Mathf.Clamp(Value + finalDelta,
                _minValue, _maxValue);
            VDebug.Log("添加 (变化量:" + delta + " + " + gainPointsModifierValue + ") * " + gainRateModifierValue + " = " +
                       finalDelta
                       + " 到 " + AttributeName + "，新数值: " + Value);
            SendEvent(Value, Value - temp, shouldPlaySFX);
        }

        public int PreviewAddTo(int delta)
        {
            if (delta == 0)
                return Value;
            var gainPointsModifierValue = VValueModifier<int>.GetModifierIntValue(gainPointsModifier, false);
            var gainRateModifierValue = VValueModifier<float>.GetModifierFloatValue(gainRateModifier, false);
            var finalDelta = (int)((delta + gainPointsModifierValue) * gainRateModifierValue);
            if (delta < 0 && finalDelta > 0)
                finalDelta = 0;
            return Value + finalDelta;
        }


        public virtual void MultiplyWith(int delta, bool shouldPlaySFX)
        {
            if (delta == 1)
                return;
            var temp = Value;
            Value = Mathf.Clamp(Value * delta, _minValue, _maxValue);
            SendEvent(Value, Value - temp, shouldPlaySFX);
        }

        protected virtual void SetValue(int value, bool shouldPlaySFX)
        {
            var delta = value - Value;
            Value = Mathf.Clamp(value, _minValue, _maxValue);
            SendEvent(Value, delta, shouldPlaySFX);
        }

        protected void SendEvent(int newValue, int delta, bool shouldPlaySFX)
        {
            var messageDict = new Dictionary<string, object>
            {
                { "NewValue", newValue },
                { "MaxValue", _maxValue },
                { "Delta", delta },
                { "shouldPlaySFX", shouldPlaySFX }
            };
            AddAdditionalEventParameters(messageDict);
            VRaisingRootEventCenter.Instance.Raise(_eventKey, messageDict);
        }
        
        protected virtual void AddAdditionalEventParameters(Dictionary<string, object> messageDict)
        {
        }

        public void AddMaxValue(int value)
        {
            _maxValue += value;
            AddTo(value, true);
            VDebug.Log("Added max value: " + value + " to " + _configuration.attributeName + ", new max value: " +
                       _maxValue);
        }
    }
}