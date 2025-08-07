using System;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;

namespace VTuber.Character.Attributes
{
    public class VMembershipCountAttribute : VCharacterAttribute
    {
        public int highestValue = 0;
        public VMembershipCountAttribute(VCharacterAttributeConfiguration configuration, int initialValue, 
            VRaisingEventKey eventKey = VRaisingEventKey.Default, int maxValue = Int32.MaxValue, int minValue = 0)
            : base(configuration, initialValue, eventKey, maxValue, minValue, false)
        {
            
        }

        protected override void SetValue(int value)
        {
            base.SetValue(value);
            if (value > highestValue)
            {
                highestValue = value;
            }
        }
        
        
    }
}