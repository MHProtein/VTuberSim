using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.UI;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VCardViewSelectionUI : VUIBehaviour, ISelectableCardMenu
    {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private TMP_Dropdown _typeDropdown;
        [SerializeField] private TMP_Dropdown _rarityDropdown;
        [SerializeField] private Toggle _isUpgraded;
        [SerializeField] private Transform grid;

        [SerializeField] private Button confirmButton;
        [SerializeField] private Button returnButton;

        private List<VSelectCardCardUI> _cardUIs; 
        private List<VSelectCardCardUI> _displayingCardUIs;
        
        private VSelectCardCardUI _selectedCardUI;

        private Action<VCard> _confirmAction;
        private Action _returnAction;

        protected override void Awake()
        {
            base.Awake();
            confirmButton.interactable = false;
            _typeDropdown.onValueChanged.AddListener(OnTypeChanged);
            _rarityDropdown.onValueChanged.AddListener(OnRarityChanged);
            _isUpgraded.onValueChanged.AddListener(OnIsUpgradedChanged);
            
            _cardUIs = new List<VSelectCardCardUI>();
            _displayingCardUIs = new List<VSelectCardCardUI>();
            confirmButton.onClick.AddListener(Confirm);
        }

        private void Confirm()
        {
            _confirmAction?.Invoke(_selectedCardUI.Card);
        }

        public void Return()
        {
            _returnAction?.Invoke();
        }

        public void Initialize(List<VCard> cards, bool select, Action<VCard> confirmAction, Action returnAction = null)
        {
            confirmButton.gameObject.SetActive(select);
            returnButton.gameObject.SetActive(!select);
            _confirmAction = confirmAction;
            _returnAction = returnAction;
            foreach (var card in cards)
            {
                var item = Instantiate(cardPrefab, grid);
                var cardItem = item.AddComponent<VSelectCardCardUI>();
                var cardUI = cardItem.GetComponent<VCardUI>();
                cardUI.SetCard(card);
                
                cardItem.Initialize(cardUI, this, select);
                _cardUIs.Add(cardItem);
                _displayingCardUIs.Add(cardItem);
            }
        }
        
        public void Close()
        {
            foreach (var cardUI in _cardUIs)
            {
                Destroy(cardUI);
            }
            _cardUIs.Clear();
            _selectedCardUI = null;
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
            List<VSelectCardCardUI> uis = new List<VSelectCardCardUI>();
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
        
        private void UpdateDisplayingCards(List<VSelectCardCardUI> newCards)
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

        public void Select(VSelectCardCardUI cardUI)
        {
            confirmButton.interactable = true;
            if (_selectedCardUI != null && _selectedCardUI == cardUI)
                return;
            
            if(_selectedCardUI is not null)
                _selectedCardUI.UnSelect();
            _selectedCardUI = cardUI;
        }
    }
}