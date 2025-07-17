using System;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;

namespace VTuber.Character.Attributes
{
    public class VAbilityAttribute : VCharacterAttribute
    {
        public VAbilityAttribute(VCharacterAttributeConfiguration configuration, int initialValue, 
            VRaisingEventKey eventKey = VRaisingEventKey.Default, int maxValue = Int32.MaxValue, 
            int minValue = 0, bool isPercentage = false)
            : base(configuration, initialValue, eventKey, maxValue, minValue, isPercentage)
        {
        }
    }
}