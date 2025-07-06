using System.Collections.Generic;
using UnityEngine;
using VTuber.Character.Attribute;
using VTuber.Core.Foundation;

namespace VTuber.Character
{
    public class VCharacterAttributeManagerConfiguration : VScriptableObject
    {
        public List<VCharacterAttributeConfiguration> attributes;
    }
}