using System;
using VTuber.Character;
using VTuber.Core.UI;

namespace Tutorial.Script
{
    public enum VTutorialWeekConditionType
    {
        Attribute,
        CardLibrarySize,
        CoopValue
    }

    [Serializable]
    public class VTutorialWeekCondition
    {
        public VTutorialWeekConditionType conditionType;
        public string idOrName;
        public VOperatorType operatorType;
        public int targetValue;

        public bool IsTrue(VCharacter character)
        {
            switch (conditionType)
            {
                case VTutorialWeekConditionType.Attribute:
                {
                    if (character.AttributeManager.TryGetAttribute(idOrName, out var attribute))
                        return VMathUtils.Compare(attribute.Value, targetValue, operatorType);
                    return false;
                }
                case VTutorialWeekConditionType.CardLibrarySize:
                {
                    return VMathUtils.Compare(character.CardLibrary.GetCardLibrarySize(), targetValue, operatorType);
                }
                case VTutorialWeekConditionType.CoopValue:
                {
                    return VMathUtils.Compare(character.CooperatorManager.GetCooperator(uint.Parse(idOrName)).CoopValue,
                        targetValue, operatorType);
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}