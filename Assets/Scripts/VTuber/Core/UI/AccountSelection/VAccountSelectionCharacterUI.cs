using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.Core.UI.VAccountSelection
{
    public class VAccountSelectionCharacterUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text maxStaminaText;
        [SerializeField] private Image initialPressureImage;
        [SerializeField] private TMP_Text initialMoneyText;
        [SerializeField] private TMP_Text initialMembershipText;
        [SerializeField] private TMP_Text initialFollowerText;
        [SerializeField] private TMP_Text singingAbilityText;
        [SerializeField] private TMP_Text singingAbilityGainEfficiencyText;
        [SerializeField] private TMP_Text gamingAbilityText;
        [SerializeField] private TMP_Text gamingAbilityGainEfficiencyText;
        [SerializeField] private TMP_Text chattingAbilityText;
        [SerializeField] private TMP_Text chattingAbilityGainEfficiencyText;
        private List<VAccountUI> _accounts;

        private VCharacter _character;
        private VCharacterConfiguration _config;


        public void SetCharacter(VCharacterConfiguration character)
        {
            _config = character;
            ApplyAccounts();
        }

        public void SetAccounts(List<VAccountUI> accounts)
        {
            _accounts = accounts;
            ApplyAccounts();
        }

        public void ApplyAccounts()
        {
            _character = new VCharacter(_config);
            if (_accounts is null)
            {
                UpdateUI();
                return;
            }

            foreach (var account in _accounts)
            foreach (var effect in account.Account.Effects)
                effect.ApplyEffect(_character, null);

            UpdateUI();
        }

        private void UpdateUI()
        {
            maxStaminaText.text = _character.AttributeManager.Attributes["CAStamina"].MaxValue.ToString();

            var pressure = _character.AttributeManager.Attributes["CAPressure"].Value;
            initialPressureImage.sprite = VUIUtils.Instance.GetPressureIcon(pressure).Value;
            initialMoneyText.text = _character.AttributeManager.Attributes["CAMoney"].Value.ToString();
            initialMembershipText.text = _character.AttributeManager.Attributes["CAMembershipCount"].Value.ToString();
            initialFollowerText.text = _character.AttributeManager.Attributes["CAFollowerCount"].Value.ToString();
            singingAbilityText.text = _character.AttributeManager.Attributes["CASingingAbility"].Value.ToString();
            gamingAbilityText.text = _character.AttributeManager.Attributes["CAGamingAbility"].Value.ToString();
            chattingAbilityText.text = _character.AttributeManager.Attributes["CAChattingAbility"].Value.ToString();
            singingAbilityGainEfficiencyText.text = "+" +
                                                    (_character.AttributeManager
                                                        .Attributes["CASingingAbilityGainEfficiency"].Value - 100) +
                                                    "%";
            gamingAbilityGainEfficiencyText.text = "+" +
                                                   (_character.AttributeManager
                                                       .Attributes["CAGamingAbilityGainEfficiency"].Value - 100) + "%";
            chattingAbilityGainEfficiencyText.text = "+" +
                                                     (_character.AttributeManager
                                                         .Attributes["CAChattingAbilityGainEfficiency"].Value - 100) +
                                                     "%";
        }
    }
}