using UnityEngine;
using VTuber.Character.Attribute;

namespace VTuber.Character.AttributeConfigurations
{
    public class VIntegerAttributeConfiguration : VCharacterAttributeConfiguration
    {
        [SerializeField] private int defaultValue;
    }
}