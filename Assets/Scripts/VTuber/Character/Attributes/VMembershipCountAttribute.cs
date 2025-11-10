using System.Collections.Generic;
using VTuber.BattleSystem.Buff;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;

namespace VTuber.Character.Attributes
{
    public class VMembershipCountAttribute : VCharacterAttribute
    {
        private readonly List<VRangeValueMap<uint>> _buffTable;
        public int highestValue;

        public VMembershipCountAttribute(VCharacterAttributeConfiguration configuration, int initialValue,
            List<VRangeValueMap<uint>> buffTable,
            VRaisingEventKey eventKey = VRaisingEventKey.Default, int maxValue = int.MaxValue, int minValue = 0)
            : base(configuration, initialValue, eventKey, maxValue, minValue)
        {
            _buffTable = buffTable;
        }

        protected override void SetValue(int value, bool shouldPlaySfx)
        {
            base.SetValue(value, shouldPlaySfx);
            if (value > highestValue) highestValue = value;
        }

        public VBuff GetBuff()
        {
            foreach (var valueMap in _buffTable)
                if (valueMap.IsInRange(Value))
                    return VDataManager.Instance.CreateBuffByID(valueMap.value);

            return null;
        }
    }
}