using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;

namespace VTuber.Character.Attributes
{
    public class VAbilityGainEfficiencyAttribute : VCharacterAttribute
    {
        public VAbilityGainEfficiencyAttribute(VCharacterAttributeConfiguration configuration,
            int initialValue, VRaisingEventKey eventKey = VRaisingEventKey.Default, int maxValue = int.MaxValue,
            int minValue = 0)
            : base(configuration, initialValue, eventKey, maxValue, minValue, true, false)
        {
        }
    }
}