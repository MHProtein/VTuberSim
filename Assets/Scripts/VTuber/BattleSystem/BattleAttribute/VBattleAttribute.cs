using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sirenix.Utilities;
using UnityEngine;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.BattleAttribute
{
    
    public class VTemporaryValue
    {
        uint _idDistributor = 0;
        Dictionary<uint, int> _tempValues = new Dictionary<uint, int>();
        
        public uint AddTemporaryValue(int value)
        {
            _tempValues.Add(_idDistributor++, value);
            return _idDistributor - 1;
        }
        
        public void RemoveTemporaryValue(uint id)
        {
            if (_tempValues.ContainsKey(id))
            {
                _tempValues.Remove(id);
            }
        }
        
        public int GetTemporaryValue()
        {
            return _tempValues.Values.Sum();
        }
        
        public void Reset()
        {
            _tempValues.Clear();
        }
    }
    
    public class VBattleAttributeSaveData
    {
        public string AttributeName;
        public string AttributeType;
        public int Value;
        public int HighestValue;
        public int MinValue;
        public int MaxValue;
        
        [JsonConverter(typeof(StringEnumConverter))]
        public VBattleEventKey eventKey;
        
        public VValueModifierSaveData<int> GainPointsModifier;
        public VValueModifierSaveData<float> GainRateModifier;
        //public VTemporaryValue TemporaryValue;
        public bool IsPercentage;
        public VColorSaveData color;
        public int defaultPlayCountPerTurn;      
        public Dictionary<string, int> scoreForAbilities;
        public int maxTurn;
    }
    
    //All the attributes treated as int type, if is percentage, it is multiplied by 100 and vice versa when used. 
    public class VBattleAttribute
    {
        public string AttributeName;
        public int Value { get; protected set; }
        public int HighestValue { get; protected set; }
        protected int _minValue;
        protected int _maxValue;
        
        public VValueModifier<float> GainRateModifier => gainRateModifier;
        protected VValueModifier<float> gainRateModifier;
        
        public VValueModifier<int> GainPointsModifier => gainPointsModifier;
        protected VValueModifier<int> gainPointsModifier;
        
        public VTemporaryValue TemporaryValue => _temporaryValue;
        protected VTemporaryValue _temporaryValue;
        
        private bool _isPercentage;
        protected VBattleEventKey _eventKey;
        
        public virtual VBattleAttributeSaveData Save()
        {
            return new VBattleAttributeSaveData
            {
                AttributeName = AttributeName,
                AttributeType = GetType().ToString(),
                Value = Value,
                HighestValue = HighestValue,
                MinValue = _minValue,
                MaxValue = _maxValue,
                eventKey = _eventKey,
                GainPointsModifier = gainPointsModifier.Save(),
                GainRateModifier = gainRateModifier.Save(),
                IsPercentage = _isPercentage
            };
        }

        public VBattleAttribute(VBattleAttributeSaveData saveData)
        {
            AttributeName = saveData.AttributeName;
            HighestValue = saveData.HighestValue;
            _minValue = saveData.MinValue;
            _maxValue = saveData.MaxValue;
            _eventKey = saveData.eventKey;
            InitSetValue(saveData.Value, false);
            gainPointsModifier = saveData.GainPointsModifier.LoadModifier(true);
            gainRateModifier = saveData.GainRateModifier.LoadModifier(true);
            _isPercentage = saveData.IsPercentage;
        }
        
        public VBattleAttribute(int value, bool isPercentage = false, VBattleEventKey eventKey = VBattleEventKey.Default, int maxValue = Int32.MaxValue, int minValue = 0)
        {
            _eventKey = eventKey;
            _minValue = minValue;
            _maxValue = maxValue;
            InitSetValue(value, false);
            _isPercentage = isPercentage;

            gainRateModifier = new VValueModifier<float>(1.0f, true);
            gainPointsModifier = new VValueModifier<int>(0, true);
            _temporaryValue = new VTemporaryValue();
        }
        
        public virtual void AddTo(int delta, bool isFromCard, bool shouldPlayTwice = false)
        {
            int gainPointsModifierValue = VValueModifier<int>.GetModifierIntValue(gainPointsModifier, true);
            float gainRateModifierValue = VValueModifier<float>.GetModifierFloatValue(gainRateModifier, true);
            int finalDelta = VMathUtils.FloatToInt((delta + gainPointsModifierValue) * (gainRateModifierValue));
            if(delta < 0 && finalDelta > 0)
                finalDelta = 0;
            SetValue(Mathf.Clamp(finalDelta + Value,
                _minValue, _maxValue), isFromCard, shouldPlayTwice); ;
            VDebug.Log("添加 (变化量:" + delta + " + " + gainPointsModifierValue + ") * " + gainRateModifierValue + " = " + finalDelta
                       + " 到 " + AttributeName + "，新数值: " + Value);
        }
        
        public int PreviewAddTo(int delta)
        {
            if (delta == 0)
                return Value;
            int gainPointsModifierValue = VValueModifier<int>.GetModifierIntValue(gainPointsModifier, false);
            float gainRateModifierValue = VValueModifier<float>.GetModifierFloatValue(gainRateModifier, false);
            int finalDelta = VMathUtils.FloatToInt((delta + gainPointsModifierValue) * (gainRateModifierValue));
            if(delta < 0 && finalDelta > 0)
                finalDelta = 0;
            return Value + finalDelta;
        }
        
        public virtual void MultiplyWith(int delta, bool isFromCard, bool shouldPlayTwice = false)
        {
            if (delta == 1)
                return;
            int temp = Value;
            Value = Mathf.Clamp(Value * delta, _minValue, _maxValue);
            SendEvent(Value, Value - temp, isFromCard, shouldPlayTwice);
        }

        protected virtual void InitSetValue(int value, bool isFromCard, bool shouldPlayTwice = false)
        {
            Value = Mathf.Clamp(value, _minValue, _maxValue);
            HighestValue = Value;
            SendEvent(Value, Value, isFromCard, shouldPlayTwice);
        }
        
        public virtual void SetValue(int value, bool isFromCard, bool shouldPlayTwice = false, bool sendEvent = true)
        {
            var delta = value - Value;
            Value = Mathf.Clamp(value, _minValue, _maxValue);
            if (Value > HighestValue)
                HighestValue = Value;
            if(sendEvent)
                SendEvent(Value, delta, isFromCard, shouldPlayTwice);
        }
        
        public virtual void SendEvent(int newValue, int delta, bool isFromCard, bool shouldPlayTwice = false)  
        {
            var messageDict = new Dictionary<string, object>
            {
                { "NewValue", newValue },
                { "Delta", delta },
                {"IsFromCard", isFromCard },
                {"ShouldPlayTwice", shouldPlayTwice },
                {"MaxValue", _maxValue}
            };
            VBattleRootEventCenter.Instance.Raise(_eventKey, messageDict);
            
            messageDict.Add("AttributeName", AttributeName);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnAttributeValueChange, messageDict);
        }

        public virtual void OnEnable()
        {
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
        }

        protected virtual void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            var removeIndices = new List<uint>();
            foreach (var mod in gainPointsModifier.Modifiers)
            {
                if (mod.Value.DecreaseTurnCount())
                {
                    removeIndices.Add(mod.Key);
                }
            }
            foreach (var index in removeIndices)
            {
                gainPointsModifier.RemoveModifier(index);
            }
            
            removeIndices = new List<uint>();
            foreach (var mod in gainRateModifier.Modifiers)
            {
                if (mod.Value.DecreaseTurnCount())
                {
                    removeIndices.Add(mod.Key);
                }
            }
            foreach (var index in removeIndices)
            {
                gainRateModifier.RemoveModifier(index);
            }
        }

        public virtual void OnDisable()
        {
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
        }
    }
}