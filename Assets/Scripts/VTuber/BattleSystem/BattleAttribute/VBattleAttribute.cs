using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VValueModifier<T>
    {
        public T DefaultValue => _defaultValue;
   
        private T _defaultValue;
        uint _idDistributor = 0;
        
        public Dictionary<uint, T> Modifiers => _modifiers;
        private Dictionary<uint, T> _modifiers = new Dictionary<uint, T>();

        public VValueModifier(T defaultValue)
        {
            this._defaultValue = defaultValue;
        }
        
        public uint AddModifier(T modifier)
        {
            _modifiers.Add(_idDistributor++, modifier);
            return _idDistributor - 1;
        }
        
        public void RemoveModifier(uint id)
        {
            if (_modifiers.ContainsKey(id))
            {
                _modifiers.Remove(id);
            }
        }
        
        public void ChangeModifier(uint id, T newValue)
        {
            if (_modifiers.ContainsKey(id))
            {
                _modifiers[id] = newValue;
            }
        }
        
        public static int GetModifierIntValue(VValueModifier<int> modifier)
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
        
        public static float GetModifierFloatValue(VValueModifier<float> modifier)
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
    
    //All the attributes treated as int type, if is percentage, it is multiplied by 100 and vice versa when used. 
    public class VBattleAttribute
    {
        public string AttributeName;
        public int Value { get; protected set; }
        protected int _minValue;
        protected int _maxValue;
        
        public VValueModifier<float> GainRateModifier => gainRateModifier;
        protected VValueModifier<float> gainRateModifier;
        
        public VValueModifier<int> GainPointsModifier => gainPointsModifier;
        protected VValueModifier<int> gainPointsModifier;
        
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
        }
        
        public virtual void AddTo(int delta, bool isFromCard, bool shouldPlayTwice = false)
        {
            if (delta == 0)
                return;
            int temp = Value;
            int gainPointsModifierValue = VValueModifier<int>.GetModifierIntValue(gainPointsModifier);
            float gainRateModifierValue = VValueModifier<float>.GetModifierFloatValue(gainRateModifier);
            int finalDelta = (int)(Value + (delta + gainPointsModifierValue) * gainRateModifierValue);
            Value = Mathf.Clamp(finalDelta,
                _minValue, _maxValue);
            VDebug.Log("添加 (变化量:" + delta + " + " + gainPointsModifierValue + ") * " + gainRateModifierValue + " = " + finalDelta
                       + " 到 " + AttributeName + "，新数值: " + Value);
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