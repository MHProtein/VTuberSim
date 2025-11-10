using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VValueModifierSaveData<T>
    {
        public T DefaultValue;

        [JsonConverter(typeof(StringEnumConverter))]
        public VBattleEventKey EventKey;

        public int ID;

        public uint idDistributor;
        public Dictionary<uint, VValueModifier<T>.ModifierItem> Modifiers;

        public VValueModifier<T> LoadModifier(bool isBattleAttribute = false)
        {
            var modifier = new VValueModifier<T>(this, isBattleAttribute);
            return modifier;
        }
    }

    [Serializable]
    public class VValueModifier<T>
    {
        private VBattleEventKey _eventKey = VBattleEventKey.Default;
        private uint _idDistributor;
        private bool _isBattleAttribute;
        public Action onModifierApply;

        [JsonConstructor]
        public VValueModifier(T defaultValue, bool isBattleAttribute = false)
        {
            DefaultValue = defaultValue;
            _isBattleAttribute = isBattleAttribute;
            AddToLookupTable();
        }

        public VValueModifier(VValueModifierSaveData<T> saveData, bool isBattleAttribute)
        {
            ID = saveData.ID;
            DefaultValue = saveData.DefaultValue;
            Modifiers = saveData.Modifiers ?? new Dictionary<uint, ModifierItem>();
            _eventKey = saveData.EventKey;
            _idDistributor = saveData.idDistributor;
            _isBattleAttribute = isBattleAttribute;
            AddToLookupTable();
        }

        public int ID { get; private set; } = -1;

        public T DefaultValue { get; }

        public Dictionary<uint, ModifierItem> Modifiers { get; } = new();

        public void AddToLookupTable()
        {
            if (!_isBattleAttribute)
                return;
            if (typeof(T) == typeof(float))
                VBattleLookUpTables.Instance.AddGainRateModifier(this as VValueModifier<float>);
            else if (typeof(T) == typeof(int))
                VBattleLookUpTables.Instance.AddGainValueModifier(this as VValueModifier<int>);
        }

        public void SetEventKey(VBattleEventKey eventKey)
        {
            _eventKey = eventKey;
        }

        public uint AddModifier(T modifier, int turnCount)
        {
            Modifiers.Add(_idDistributor++, new ModifierItem(modifier, turnCount));
            SendEvent();
            return _idDistributor - 1;
        }

        public void RemoveModifier(uint id)
        {
            if (Modifiers.ContainsKey(id)) Modifiers.Remove(id);
            SendEvent();
        }

        public void ChangeModifier(uint id, T newValue)
        {
            if (Modifiers.ContainsKey(id)) Modifiers[id].SetValue(newValue);
            SendEvent();
        }

        public static int GetModifierIntValue(VValueModifier<int> modifier, bool addValue)
        {
            if (modifier.Modifiers.Count == 0)
                return modifier.DefaultValue;
            var total = modifier.DefaultValue;
            foreach (var mod in modifier.Modifiers) total += mod.Value.Value;
            if (addValue)
                modifier.onModifierApply?.Invoke();
            return total;
        }

        public static float GetModifierFloatValue(VValueModifier<float> modifier, bool addValue)
        {
            if (modifier.Modifiers.Count == 0)
                return modifier.DefaultValue;
            var total = modifier.DefaultValue;
            foreach (var mod in modifier.Modifiers) total += mod.Value.Value;
            if (addValue)
                modifier.onModifierApply?.Invoke();
            return total;
        }

        public void Reset()
        {
            Modifiers.Clear();
            SendEvent();
        }

        public void SendEvent()
        {
            VBattleRootEventCenter.Instance.Raise(_eventKey, new Dictionary<string, object>());
        }

        public void SetID(int idDistributor)
        {
            ID = idDistributor;
        }

        public VValueModifierSaveData<T> Save()
        {
            return new VValueModifierSaveData<T>
            {
                ID = ID,
                DefaultValue = DefaultValue,
                Modifiers = Modifiers,
                EventKey = _eventKey,
                idDistributor = _idDistributor
            };
        }

        public class ModifierItem
        {
            public ModifierItem(T value, int turnCount)
            {
                Value = value;
                TurnCount = turnCount;
            }

            public T Value { get; private set; }

            public int TurnCount { get; private set; }

            public void SetValue(T value)
            {
                Value = value;
            }

            public bool DecreaseTurnCount()
            {
                if (TurnCount == -1)
                    return false;
                TurnCount--;
                return TurnCount <= 0;
            }
        }
    }
}