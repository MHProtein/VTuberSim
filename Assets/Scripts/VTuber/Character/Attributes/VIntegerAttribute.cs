using UnityEngine;
using VTuber.Character.Attribute;

namespace VTuber.Character.Attributes
{
    public class VIntegerAttribute : VCharacterAttributeConfiguration
    {
        [SerializeField] private int defaultValue;
    }
}