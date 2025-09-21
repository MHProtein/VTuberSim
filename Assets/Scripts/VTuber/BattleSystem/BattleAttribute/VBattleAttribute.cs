using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.BattleAttribute
{
    [Serializable]
    public class VValueModifier<T>
    {
        public class ModifierItem
        {
            public T Value => _value;
            private T _value;
            public int TurnCount => _turnCount;
            private int _turnCount;

            public ModifierItem(T value, int turnCount)
            {
                _value = value;
                _turnCount = turnCount;
            }

            public void SetValue(T value)
            {
                _value = value;
            }

            public bool DecreaseTurnCount()
            {
                if (_turnCount == -1)
                    return false;
                _turnCount--;
                return _turnCount <= 0;
            }
            
        }
        
        public T DefaultValue => _defaultValue;
   
        private T _defaultValue;
        uint _idDistributor = 0;
        
        public Dictionary<uint, ModifierItem> Modifiers => _modifiers;
        private Dictionary<uint, ModifierItem> _modifiers = new Dictionary<uint, ModifierItem>();

        private VBattleEventKey _eventKey = VBattleEventKey.Default;
        
        public VValueModifier(T defaultValue)
        {
            this._defaultValue = defaultValue;
        }
        
        public void SetEventKey(VBattleEventKey eventKey)
        {
            _eventKey = eventKey;
        }
        
        public uint AddModifier(T modifier, int turnCount)
        {
            _modifiers.Add(_idDistributor++, new ModifierItem(modifier, turnCount));
            SendEvent();
            return _idDistributor - 1;
        }
        
        public void RemoveModifier(uint id)
        {
            if (_modifiers.ContainsKey(id))
            {
                _modifiers.Remove(id);
            }
            SendEvent();
        }
        
        public void ChangeModifier(uint id, T newValue)
        {
            if (_modifiers.ContainsKey(id))
            {
                _modifiers[id].SetValue(newValue);
            }
            SendEvent();
        }
        
        public static int GetModifierIntValue(VValueModifier<int> modifier)
        {
            if (modifier.Modifiers.Count == 0)
                return modifier.DefaultValue;
            int total = modifier.DefaultValue;
            foreach (var mod in modifier.Modifiers)
            {
                total += mod.Value.Value;
            }
            return total;
        }
        
        public static float GetModifierFloatValue(VValueModifier<float> modifier)
        {
            if (modifier.Modifiers.Count == 0)
                return modifier.DefaultValue;
            float total = modifier.DefaultValue;
            foreach (var mod in modifier.Modifiers)
            {
                total += mod.Value.Value;
            }
            return total;
        }

        public void Reset()
        {
            _modifiers.Clear();
            SendEvent();
        }
        
        public void SendEvent()
        {
            VBattleRootEventCenter.Instance.Raise(_eventKey, new Dictionary<string, object>());
        }
    }
    
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
    
    //All the attributes treated as int type, if is percentage, it is multiplied by 100 and vice versa when used. 
    public class VBattleAttribute
    {
        public string AttributeName;
        public int Value { get; private set; }
        public int HighestValue { get; private set; }
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
        
        public VBattleAttribute(int value, bool isPercentage = false, VBattleEventKey eventKey = VBattleEventKey.Default, int maxValue = Int32.MaxValue, int minValue = 0)
        {
            _eventKey = eventKey;
            _minValue = minValue;
            _maxValue = maxValue;
            InitSetValue(value, false);
            _isPercentage = isPercentage;

            gainRateModifier = new VValueModifier<float>(1.0f);
            gainPointsModifier = new VValueModifier<int>(0);
            _temporaryValue = new VTemporaryValue();
        }
        
        public virtual void AddTo(int delta, bool isFromCard, bool shouldPlayTwice = false)
        {
            int gainPointsModifierValue = VValueModifier<int>.GetModifierIntValue(gainPointsModifier);
            float gainRateModifierValue = VValueModifier<float>.GetModifierFloatValue(gainRateModifier);
            int finalDelta = VMathUtils.FloatToInt((delta + gainPointsModifierValue) * (gainRateModifierValue ));
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
            int gainPointsModifierValue = VValueModifier<int>.GetModifierIntValue(gainPointsModifier);
            float gainRateModifierValue = VValueModifier<float>.GetModifierFloatValue(gainRateModifier);
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