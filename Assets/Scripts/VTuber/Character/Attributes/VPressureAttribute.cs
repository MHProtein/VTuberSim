using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Buff;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;

namespace VTuber.Character.Attributes
{
    public class VPressureAttribute : VCharacterAttribute
    {
        List<int> _buffTable;
        List<VRaisingEffect> _effects;
        public VPressureAttribute(VCharacterAttributeConfiguration configuration, List<int> buffTable, Dictionary<uint, string> effects, int initialValue,
            VRaisingEventKey eventKey = VRaisingEventKey.Default, 
            int maxValue = Int32.MaxValue, int minValue = 0, bool isPercentage = false)
            : base(configuration, initialValue, eventKey, maxValue, minValue, isPercentage, false)
        {
            _buffTable = buffTable;
            _effects = new List<VRaisingEffect>();
            foreach (var effect in effects)
            {
                _effects.Add(VDataManager.Instance.CreateRaisingEffectByID(effect.Key, effect.Value, effect.Value));
            }
        }

        public VBuff GetBuff()
        {
            return VDataManager.Instance.CreateBuffByID((uint)_buffTable[Value - 1]);
        }
        
        public void ApplyEffects(VCharacter character)
        {
            foreach (var effect in _effects)
            {
                effect.ApplyEffect(character, null);
            }
        }
        
    }
}