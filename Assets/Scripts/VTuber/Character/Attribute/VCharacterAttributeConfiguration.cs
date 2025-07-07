using System;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;
using VTuber.Core.TypeSerialization;
using VTuber.Core.TypeSerialization;
namespace VTuber.Character.Attribute
{


    public class VCharacterAttributeConfiguration : VScriptableObject
    {
        [StringToEnum] public string attributeName;
        
        public string description;
        
        [Space(5)]
        [Core.TypeSerialization.TypeFilter(typeof(VCharacterAttribute))] public SerializableType attribute;
        
        public bool isConvertToBattleAttribute = true;
        
        [ShowIf("isConvertToBattleAttribute")]
        [Core.TypeSerialization.TypeFilter(typeof(VBattleAttribute))] 
        public SerializableType battleAttribute;
        
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
