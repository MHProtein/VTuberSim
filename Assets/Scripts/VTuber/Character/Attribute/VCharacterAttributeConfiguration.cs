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
        public string attributeName;
        
        [TextArea] public string description;
        
        [Space(5)]
        [Header("Battle Attribute")]
        public bool isConvertToBattleAttribute = true;
        [ShowIf("isConvertToBattleAttribute")]
        [ShowIf("isConvertToBattleAttribute")][Core.TypeSerialization.TypeFilter(typeof(VBattleAttribute))] 
        public SerializableType battleAttribute;
        [ShowIf("isConvertToBattleAttribute")][StringToEnum("BattleAttributes")] public string battleAttributeName;
        [ShowIf("isConvertToBattleAttribute")] public bool isBattleAttributePercentage = false;
        [ShowIf("isConvertToBattleAttribute")] public VBattleEventKey battleEventKey = VBattleEventKey.Default;
        [ShowIf("isConvertToBattleAttribute")] public int minValue = 0;
        [ShowIf("isConvertToBattleAttribute")] public int maxValue = int.MaxValue;
    }
}
