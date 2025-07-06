using System;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.Core.TypeSerialization;

namespace VTuber.Character.Attribute
{
    public class VCharacterAttributeConfiguration : VScriptableObject
    {
        public string attributeName;
        public string description;
        [Space(5)]
        [TypeFilter(typeof(VCharacterAttribute))] public SerializableType attribute;
        
        public VCharacterAttribute GetAttribute()
        {
            if (attribute == null)
            {
                throw new InvalidOperationException("Attribute type is not set.");
            }

            return (VCharacterAttribute)Activator.CreateInstance(attribute.GetType());
        }
    }
}
