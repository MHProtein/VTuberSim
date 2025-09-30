using TMPro;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.Core.UI
{
    public class VAbilityDetails : VUIBehaviour
    {
        [SerializeField] private TMP_Text singingAbilityValueText;
        [SerializeField] private TMP_Text singingAbilityGainEfficiencyText;
        [SerializeField] private TMP_Text gamingAbilityText;
        [SerializeField] private TMP_Text gamingAbilityGainEfficiencyText;
        [SerializeField] private TMP_Text chattingAbilityText;
        [SerializeField] private TMP_Text chattingAbilityGainEfficiencyText;

        public void SetDetails(VCharacter character)
        {
            singingAbilityValueText.text = character.AttributeManager.TryGetAttributeValue("CASingingAbility", out int value, out bool isPercentage) ? value.ToString() : "0";
            gamingAbilityText.text = character.AttributeManager.TryGetAttributeValue("CAGamingAbility", out value, out isPercentage) ? value.ToString() : "0";
            chattingAbilityText.text = character.AttributeManager.TryGetAttributeValue("CAChattingAbility", out value, out isPercentage) ? value.ToString() : "0";
            singingAbilityGainEfficiencyText.text = "+" + VMathUtils.GetPercentage(character.AttributeManager.TryGetAttributeValue
                ("CASingingAbilityGainEfficiency", out value, out isPercentage) ? value : 0, 1) + "%";
            gamingAbilityGainEfficiencyText.text = "+" + VMathUtils.GetPercentage(character.AttributeManager.TryGetAttributeValue
                ("CAGamingAbilityGainEfficiency", out value, out isPercentage) ? value : 0, 1) + "%";
            chattingAbilityGainEfficiencyText.text = "+" + VMathUtils.GetPercentage(character.AttributeManager.TryGetAttributeValue
                ("CAChattingAbilityGainEfficiency", out value, out isPercentage) ? value : 0, 1) + "%";
        }

        public void SetDetails(VCharacterConfiguration characterConfig)
        {
            singingAbilityValueText.text = characterConfig.singingAbilityInitialValue.ToString();
            gamingAbilityText.text = characterConfig.gamingAbilityInitialValue.ToString();
            chattingAbilityText.text = characterConfig.chattingAbilityInitialValue.ToString();
            singingAbilityGainEfficiencyText.text = "+" + VMathUtils.GetPercentage(characterConfig.singingAbilityGainEfficiencyInitialValue, 1) + "%";
            gamingAbilityGainEfficiencyText.text = "+" + VMathUtils.GetPercentage(characterConfig.gamingAbilityGainEfficiencyInitialValue, 1) + "%";
            chattingAbilityGainEfficiencyText.text = "+" + VMathUtils.GetPercentage(characterConfig.chattingAbilityGainEfficiencyInitialValue, 1) + "%";
        }

        public void SetDetails(VCharacterSaveData characterSaveData)
        {
            var attributeSaveData = characterSaveData.attributes;

            singingAbilityValueText.text = attributeSaveData.Find(data => data.attributeName == "CASingingAbility").value.ToString();
            gamingAbilityText.text = attributeSaveData.Find(data => data.attributeName == "CAGamingAbility").value.ToString();
            chattingAbilityText.text = attributeSaveData.Find(data => data.attributeName == "CAChattingAbility").value.ToString();
            singingAbilityGainEfficiencyText.text = "+" + VMathUtils.GetPercentage(attributeSaveData.Find(data => data.attributeName == "CASingingAbilityGainEfficiency").value, 1) + "%";
            gamingAbilityGainEfficiencyText.text = "+" + VMathUtils.GetPercentage(attributeSaveData.Find(data => data.attributeName == "CAGamingAbilityGainEfficiency").value, 1) + "%";
            chattingAbilityGainEfficiencyText.text = "+" + VMathUtils.GetPercentage(attributeSaveData.Find(data => data.attributeName == "CAChattingAbilityGainEfficiency").value, 1) + "%";
            
        }
        
    }
}