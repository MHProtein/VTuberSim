using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;
using VTuber.Core.TypeSerialization;
namespace VTuber.Character.Attribute
{


    public class VCharacterAttributeConfiguration : VScriptableObject
    {
        [Header("Attribute Configuration")]
        [StringToEnum("CharacterAttributes")] public string attributeName;
        
        [TextArea] public string description;
        
        public bool isPercentage = false;
        
        [Space(5)]
        [Header("Battle Attribute")]
        public bool isConvertToBattleAttribute = true;
        [ShowIf("isConvertToBattleAttribute")]
        [Core.TypeSerialization.TypeFilter(typeof(VBattleAttribute))] 
        public SerializableType battleAttribute;
        [StringToEnum("BattleAttributes")] public string battleAttributeName;
        public bool isBattleAttributePercentage = false;
        public VBattleEventKey battleEventKey = VBattleEventKey.Default;
        public int minValue = 0;
        public int maxValue = int.MaxValue;
    }
}
