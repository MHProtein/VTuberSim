using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.UI;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VCardLibraryUI : VUIBehaviour
    {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private TMP_Dropdown _typeDropdown;
        [SerializeField] private TMP_Dropdown _rarityDropdown;
        [SerializeField] private Toggle _isUpgraded;
        [SerializeField] private Transform grid;

        private List<VCardUI> _cardUIs; 
        private List<VCardUI> _displayingCardUIs;
        

        protected override void Awake()
        {
            base.Awake();
            
            _typeDropdown.onValueChanged.AddListener(OnTypeChanged);
            _rarityDropdown.onValueChanged.AddListener(OnRarityChanged);
            _isUpgraded.onValueChanged.AddListener(OnIsUpgradedChanged);
            
            _cardUIs = new List<VCardUI>();
            _displayingCardUIs = new List<VCardUI>();
        }
        
        public void Initialize(List<VCard> cards)
        {
            foreach (var card in cards)
            {
                var item = Instantiate(cardPrefab, grid);
                var cardItem = item.GetComponent<VCardUI>();
                if (cardItem != null)
                {
                    cardItem.SetCard(card);
                    _cardUIs.Add(cardItem);
                    _displayingCardUIs.Add(cardItem);
                }
            }
        }
        
        public void Close()
        {
            foreach (var cardUI in _cardUIs)
            {
                Destroy(cardUI);
            }
            _cardUIs.Clear();
            _displayingCardUIs.Clear();
            _typeDropdown.value = 0; // Reset to "All"
            _rarityDropdown.value = 0; // Reset to "Common"
            _isUpgraded.isOn = false; // Reset to unchecked
        }
        
        public void OnTypeChanged(int value)
        {
            OnValueChanged();
        }
        
        public void OnRarityChanged(int value)
        {
            OnValueChanged();
        }

        private void OnValueChanged()
        {
            List<VCardUI> uis = new List<VCardUI>();
            string type = _typeDropdown.options[_typeDropdown.value].text;
            uis.AddRange(type == "All" ? _cardUIs : _cardUIs.Where((ui => ui.Card.CardType == type)));
            string rarityStr = _rarityDropdown.options[_rarityDropdown.value].text;
            if (!rarityStr.Equals("All"))
            {
                VCardRarity rarity = Enum.Parse<VCardRarity>(rarityStr);
                uis = uis.Where((ui => ui.Card.Rarity == rarity)).ToList();
            }
            UpdateDisplayingCards(uis);
        }
        
        private void UpdateDisplayingCards(List<VCardUI> newCards)
        {
            foreach (var cardUI in _displayingCardUIs)
            {
                cardUI.transform.SetParent(null);
            }
            _displayingCardUIs = newCards;
            foreach (var cardUI in _displayingCardUIs)
            {
                cardUI.transform.SetParent(grid);
            }
        }
        
        public void OnIsUpgradedChanged(bool value)
        {
            Debug.Log($"Is Upgraded: {value}");
        }


    }
}