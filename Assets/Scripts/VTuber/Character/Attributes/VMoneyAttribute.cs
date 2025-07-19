using System;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;

namespace VTuber.Character.Attributes
{
    public class VMoneyAttribute : VCharacterAttribute
    {
        public VMoneyAttribute(VCharacterAttributeConfiguration configuration, int initialValue, 
            VRaisingEventKey eventKey = VRaisingEventKey.Default, 
            int maxValue = Int32.MaxValue, int minValue = 0)
            : base(configuration, initialValue, eventKey, maxValue, minValue)
        {
            
        }
    }
}