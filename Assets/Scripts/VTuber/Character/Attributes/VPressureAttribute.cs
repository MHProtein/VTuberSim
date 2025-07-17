using System;
using System.Collections.Generic;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;

namespace VTuber.Character.Attributes
{
    public class VPressureAttribute : VCharacterAttribute
    {
        Dictionary<int, int> _buffTable;
        
        public VPressureAttribute(VCharacterAttributeConfiguration configuration, Dictionary<int, int> buffTable, int initialValue,
            VRaisingEventKey eventKey = VRaisingEventKey.Default, 
            int maxValue = Int32.MaxValue, int minValue = 0, bool isPercentage = false)
            : base(configuration, initialValue, eventKey, maxValue, minValue, isPercentage)
        {
            _buffTable = buffTable;
        }
    }
}