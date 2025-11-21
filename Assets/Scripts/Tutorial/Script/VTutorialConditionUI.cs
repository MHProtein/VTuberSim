using TMPro;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.Foundation;

namespace Tutorial.Script
{
    public class VTutorialConditionUI : VUIBehaviour
    {
        [SerializeField] TMP_Text conditionText;
        
        public void SetCondition(VTutorialWeekCondition condition, VCharacter character)
        {
            conditionText.text = condition.GetConditionDescription(character);
        }
    }
}