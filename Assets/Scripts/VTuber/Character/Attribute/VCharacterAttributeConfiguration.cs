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
        [Header("Attribute Configuration")]
        [StringToEnum("CharacterAttributes")] public string attributeName;
        
        [TextArea] public string description;
        
        [Space(5)]
        [Core.TypeSerialization.TypeFilter(typeof(VCharacterAttribute))] public SerializableType attribute;
        
        public int defaultValue ;
        public bool isPercentage = false;
        
        [Space(5)]
        [Header("Battle Attribute")]
        public bool isConvertToBattleAttribute = true;
        [ShowIf("isConvertToBattleAttribute")]
        [Core.TypeSerialization.TypeFilter(typeof(VBattleAttribute))] 
        public SerializableType battleAttribute;
        [StringToEnum("BattleAttributes")] public string battleAttributeName;
        public bool isBattleAttributePercentage = false;
        
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
