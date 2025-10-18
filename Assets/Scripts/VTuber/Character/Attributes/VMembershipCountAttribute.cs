using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Buff;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;

namespace VTuber.Character.Attributes
{
    public class VMembershipCountAttribute : VCharacterAttribute
    {
        public int highestValue = 0;
        private List<VRangeValueMap<uint>> _buffTable;
        public VMembershipCountAttribute(VCharacterAttributeConfiguration configuration, int initialValue, List<VRangeValueMap<uint>> buffTable,
            VRaisingEventKey eventKey = VRaisingEventKey.Default, int maxValue = Int32.MaxValue, int minValue = 0)
            : base(configuration, initialValue, eventKey, maxValue, minValue, false)
        {
            this._buffTable = buffTable;
        }

        protected override void SetValue(int value, bool shouldPlaySfx)
        {
            base.SetValue(value, shouldPlaySfx);
            if (value > highestValue)
            {
                highestValue = value;
            }
        }
        
        public VBuff GetBuff()
        {
            foreach (var valueMap in _buffTable)
            {
                if (valueMap.IsInRange(Value))
                {
                    return VDataManager.Instance.CreateBuffByID(valueMap.value);
                }
            }

            return null;
        }
    }
}