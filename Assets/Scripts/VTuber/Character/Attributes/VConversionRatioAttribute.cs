using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;

namespace VTuber.Character.Attributes
{
    public class VConversionRatioAttribute : VCharacterAttribute
    {
        public float fraction;

        public VConversionRatioAttribute(VCharacterAttributeConfiguration configuration, float initialValue,
            VRaisingEventKey eventKey, int maxValue, int minValue)
            : base(configuration, (int)initialValue, eventKey, maxValue, minValue, true, false)
        {
            fraction = initialValue - (int)initialValue;
        }

        public float GetValue()
        {
            return (Value + fraction) / 100.0f;
        }
    }
}