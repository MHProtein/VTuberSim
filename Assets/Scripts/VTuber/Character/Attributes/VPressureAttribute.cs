using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Buff;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;

namespace VTuber.Character.Attributes
{
    public class VPressureAttribute : VCharacterAttribute
    {
        List<int> _buffTable;
        
        public VPressureAttribute(VCharacterAttributeConfiguration configuration, List<int> buffTable, int initialValue,
            VRaisingEventKey eventKey = VRaisingEventKey.Default, 
            int maxValue = Int32.MaxValue, int minValue = 0, bool isPercentage = false)
            : base(configuration, initialValue, eventKey, maxValue, minValue, isPercentage, false)
        {
            _buffTable = buffTable;
        }

        public VBuff GetBuff()
        {
            return VResourcesManager.Instance.CreateBuffByID((uint)_buffTable[Value - 1]);
        }
        
    }
}