using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.UI;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.BattleSystem.Core.UI.VCharacterSelection
{
    public class VCharacterDetailsUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text liveTypeText;
        [SerializeField] private VCardUI card;
        [SerializeField] private TMP_Text staminaText;
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text followerText;
        [SerializeField] private TMP_Text singingAbilityValueText;
        [SerializeField] private TMP_Text singingAbilityGainEfficiencyText;
        [SerializeField] private TMP_Text gamingAbilityText;
        [SerializeField] private TMP_Text gamingAbilityGainEfficiencyText;
        [SerializeField] private TMP_Text chattingAbilityText;
        [SerializeField] private TMP_Text chattingAbilityGainEfficiencyText;

        private Dictionary<uint, VCard> cardsCreated = new Dictionary<uint, VCard>();
        
        public string GetLiveTypeText(string liveType)
        {
            switch (liveType)
            {
                case "A":
                    return "情感系";
                case "I":
                    return "知性系";
                case "G":
                    return "重力系";
                case "C":
                    return "混沌系";
                case "P":
                    return "精神系";
            }

            return null;
        }
        
        public string GetPercentage(int value, int decimalPlaces)
        {
            var v = (value - 100f);
            return v.ToString();
        }
        
        public void SetDetails(VCharacterConfiguration characterConfig)
        {
            nameText.text = characterConfig.characterName;
            descriptionText.text = characterConfig.description;
            liveTypeText.text = GetLiveTypeText(characterConfig.liveType);
            if(cardsCreated.TryGetValue(characterConfig.initialCardId, out var value))
                card.SetCard(value);
            else
            {
                var c = VDataManager.Instance.CreateCardByID(characterConfig.initialCardId);
                cardsCreated.Add(characterConfig.initialCardId, c);
                card.SetCard(c);
            }

            staminaText.text = $"{characterConfig.staminaInitialValue}/{characterConfig.staminaMaxValue}";
            moneyText.text = characterConfig.moneyInitialValue.ToString();
            followerText.text = characterConfig.followerCountInitialValue.ToString();
            singingAbilityValueText.text = characterConfig.singingAbilityInitialValue.ToString();
            gamingAbilityText.text = characterConfig.gamingAbilityInitialValue.ToString();
            chattingAbilityText.text = characterConfig.chattingAbilityInitialValue.ToString();
            singingAbilityGainEfficiencyText.text = "+" + GetPercentage(characterConfig.singingAbilityGainEfficiencyInitialValue, 1) + "%";
            gamingAbilityGainEfficiencyText.text = "+" + GetPercentage(characterConfig.gamingAbilityGainEfficiencyInitialValue, 1) + "%";
            chattingAbilityGainEfficiencyText.text = "+" + GetPercentage(characterConfig.chattingAbilityGainEfficiencyInitialValue, 1) + "%";
        }
    }
}