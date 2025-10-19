using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.BattleAttribute
{
public class VValueModifierSaveData<T>
    {
        public int ID;
        public T DefaultValue;
        public Dictionary<uint, VValueModifier<T>.ModifierItem> Modifiers;
        [JsonConverter(typeof(StringEnumConverter))]
        public VBattleEventKey EventKey;
        public uint idDistributor;

        public VValueModifier<T> LoadModifier(bool isBattleAttribute = false)
        {
            var modifier = new VValueModifier<T>(this, isBattleAttribute);
            return modifier;
        }
    }
    
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
        
        public int ID => _id;
        private int _id = -1;
        public Action onModifierApply;
        public T DefaultValue => _defaultValue;
   
        private T _defaultValue;
        uint _idDistributor = 0;
        
        public Dictionary<uint, ModifierItem> Modifiers => _modifiers;
        private Dictionary<uint, ModifierItem> _modifiers = new Dictionary<uint, ModifierItem>();

        private VBattleEventKey _eventKey = VBattleEventKey.Default;
        private bool _isBattleAttribute = false;
        
        [JsonConstructor]
        public VValueModifier(T defaultValue, bool isBattleAttribute = false)
        {
            this._defaultValue = defaultValue;
            _isBattleAttribute = isBattleAttribute;
            AddToLookupTable();
        }

        public VValueModifier(VValueModifierSaveData<T> saveData, bool isBattleAttribute)
        {
            _id = saveData.ID;
            _defaultValue = saveData.DefaultValue;
            _modifiers = saveData.Modifiers ?? new Dictionary<uint, ModifierItem>();
            _eventKey = saveData.EventKey;
            _idDistributor = saveData.idDistributor;
            _isBattleAttribute = isBattleAttribute;
            AddToLookupTable();
        }

        public void AddToLookupTable()
        {
            if (!_isBattleAttribute)
                return;
            if (typeof(T) == typeof(float))
            {
                VBattleLookUpTables.Instance.AddGainRateModifier(this as VValueModifier<float>);
            }
            else if (typeof(T) == typeof(int))
            {
                VBattleLookUpTables.Instance.AddGainValueModifier(this as VValueModifier<int>);
            }
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
        
        public static int GetModifierIntValue(VValueModifier<int> modifier, bool addValue)
        {
            if (modifier.Modifiers.Count == 0)
                return modifier.DefaultValue;
            int total = modifier.DefaultValue;
            foreach (var mod in modifier.Modifiers)
            {
                total += mod.Value.Value;
            }
            if(addValue)
                modifier.onModifierApply?.Invoke();
            return total;
        }
        
        public static float GetModifierFloatValue(VValueModifier<float> modifier, bool addValue)
        {
            if (modifier.Modifiers.Count == 0)
                return modifier.DefaultValue;
            float total = modifier.DefaultValue;
            foreach (var mod in modifier.Modifiers)
            {
                total += mod.Value.Value;
            }
            if(addValue)
                modifier.onModifierApply?.Invoke();
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

        public void SetID(int idDistributor)
        {
            _id = idDistributor;
        }

        public VValueModifierSaveData<T> Save()
        {
            return new VValueModifierSaveData<T>
            {
                ID = _id,
                DefaultValue = _defaultValue,
                Modifiers = _modifiers,
                EventKey = _eventKey,
                idDistributor = _idDistributor
            };
        }
    }
}