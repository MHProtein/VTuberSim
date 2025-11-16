using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.UI;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.SE;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.ScheduleSystem.UI
{
    public class VSelectCardMenuAnimation : VRaisingAnimation, ISelectableCardMenu
    {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private TMP_Dropdown _typeDropdown;
        [SerializeField] private TMP_Dropdown _rarityDropdown;
        [SerializeField] private Toggle _isUpgraded;
        [SerializeField] private Transform grid;
        [SerializeField] private VCardUI previewCardUI;

        [SerializeField] public Button confirmButton;
        [SerializeField] public Button returnButton;

        private List<VSelectCardCardUI> _cardUIs;

        private List<VSelectCardCardUI> _displayingCardUIs;
        // Simple object pool for card UI instances
        private Queue<VSelectCardCardUI> _cardPool = new();
        private Action<VCard> _previewAction;
        private Action<VCard> _confirmAction;
        private Action _returnAction;
        private VAnimationType _cardSelectAnimationType;

        private VSelectCardCardUI _selectedCardUI;

        private Action _onComplete;

        protected override void Awake()
        {
            base.Awake();
            confirmButton.interactable = false;
            _typeDropdown.onValueChanged.AddListener(OnTypeChanged);
            _rarityDropdown.onValueChanged.AddListener(OnRarityChanged);
            _isUpgraded.onValueChanged.AddListener(OnIsUpgradedChanged);

            _cardUIs = new List<VSelectCardCardUI>();
            _displayingCardUIs = new List<VSelectCardCardUI>();
            _cardPool = new Queue<VSelectCardCardUI>();
            confirmButton.onClick.AddListener(Confirm);
            
            if (previewCardUI)
                previewCardUI.gameObject.SetActive(false);

            for (int i = 0; i < 30; i++)
            {
                var obj = Instantiate(cardPrefab, ui.transform);
                var cardItem = obj.GetComponent<VSelectCardCardUI>();
                cardItem.Initialize(cardItem.GetComponentInChildren<VCardUI>(), this, false);
                obj.SetActive(false);
                _cardPool.Enqueue(cardItem);
            }
        }

        public void Select(VSelectCardCardUI cardUI)
        {
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
            confirmButton.interactable = true;
            if (_selectedCardUI != null && _selectedCardUI == cardUI)
                return;

            if (_selectedCardUI is not null)
                _selectedCardUI.UnSelect();
            _selectedCardUI = cardUI;

            if (previewCardUI)
            {
                previewCardUI.gameObject.SetActive(true);
                var previewCard = VDataManager.Instance.CreateCardByID(_selectedCardUI.Card.configID);
                _previewAction?.Invoke(previewCard);
                previewCardUI.SetCard(previewCard);
            }
        }

        private void Confirm()
        {
            var position = _selectedCardUI.transform.position;
            _selectedCardUI.transform.position = position;
            
            _confirmAction?.Invoke(_selectedCardUI.Card);
            switch (_cardSelectAnimationType)
            {
                case VAnimationType.AddCard:
                    VRaisingAnimationSystem.Instance.EnqueueAnimationRequest(
                        VAnimationRequestFactory.CreateAddCardRequest(_selectedCardUI.Card), true);
                    break;
                case VAnimationType.RemoveCard:
                    VRaisingAnimationSystem.Instance.EnqueueAnimationRequest(
                        VAnimationRequestFactory.CreateRemoveCardRequest(_selectedCardUI.Card), true);
                    break;
                case VAnimationType.UpgradeCard:
                    VRaisingAnimationSystem.Instance.EnqueueAnimationRequest(
                        VAnimationRequestFactory.CreateUpgradeCardRequest(_selectedCardUI.Card), true);
                    break;
            }
        
            _onComplete?.Invoke();
        }

        public void Return()
        {
            _returnAction?.Invoke();
            _onComplete?.Invoke();
        }

        public override void BeginAnimation(VAnimationRequest request, Action onComplete, bool isLastSameType)
        {
            base.BeginAnimation(request, onComplete, isLastSameType);
            
            _onComplete = onComplete;
            Initialize(request.cards, request.returnable, request.cardSelectable, request.cardSelectAnimationType, request.cardSelectConfirmAction,
                request.cardSelectReturnAction, request.cardSelectPreviewAction);
        }

        public void Initialize(List<VCard> cards, bool returnable, bool selectable, VAnimationType cardSelectAnimationType, Action<VCard> cardSelectConfirmAction,
            Action returnAction = null, Action<VCard> previewAction = null)
        {
            confirmButton.gameObject.SetActive(selectable);
            returnButton.gameObject.SetActive(!selectable);
            if (returnable)
                returnButton.gameObject.SetActive(true);
            _returnAction = returnAction;
            _previewAction = previewAction;
            _confirmAction = cardSelectConfirmAction;
            _cardSelectAnimationType = cardSelectAnimationType;
            foreach (var card in cards)
            {
                var cardItem = GetPooledCard();
                cardItem.transform.SetParent(grid, false);
                cardItem.SetCard(card, selectable);
                _cardUIs.Add(cardItem);
                _displayingCardUIs.Add(cardItem);
                cardItem.Popup();
            }
        }

        public override void ResetAnimation()
        {
            base.ResetAnimation();
            if(_selectedCardUI)
                _selectedCardUI.transform.SetParent(grid);
            foreach (var cardUI in _cardUIs) ReturnCardToPool(cardUI);
            _cardUIs.Clear();
            _selectedCardUI = null;
            _displayingCardUIs.Clear();
            _typeDropdown.value = 0; 
            _rarityDropdown.value = 0; 
            _isUpgraded.isOn = false;
            if (previewCardUI)
                previewCardUI.gameObject.SetActive(false);
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
            var uis = new List<VSelectCardCardUI>();
            var type = _typeDropdown.options[_typeDropdown.value].text;
            uis.AddRange(type == "All" ? _cardUIs : _cardUIs.Where(ui => ui.Card.CardType == type));
            var rarityStr = _rarityDropdown.options[_rarityDropdown.value].text;
            if (!rarityStr.Equals("All"))
            {
                var rarity = Enum.Parse<VCardRarity>(rarityStr);
                uis = uis.Where(ui => ui.Card.Rarity == rarity).ToList();
            }

            UpdateDisplayingCards(uis);
        }
        
        public void OnIsUpgradedChanged(bool value)
        {
            var uis = _cardUIs.Where(ui => ui.Card.IsUpgraded == value).ToList();
            UpdateDisplayingCards(uis);
        }

        private void UpdateDisplayingCards(List<VSelectCardCardUI> newCards)
        {
            foreach (var cardUI in _displayingCardUIs) cardUI.transform.SetParent(null);
            _displayingCardUIs = newCards;
            foreach (var cardUI in _displayingCardUIs) cardUI.transform.SetParent(grid);
            foreach (var cardUI in _displayingCardUIs) cardUI.Popup();
        }

        private VSelectCardCardUI GetPooledCard()
        {
            if (_cardPool.Count > 0)
            {
                var pooled = _cardPool.Dequeue();
                pooled.gameObject.SetActive(true);
                return pooled;
            }

            var obj = Instantiate(cardPrefab);
            var cardItem = obj.AddComponent<VSelectCardCardUI>();
            return cardItem;
        }

        private void ReturnCardToPool(VSelectCardCardUI card)
        {
            if (card == null) return;
            card.transform.SetParent(null);
            card.gameObject.SetActive(false);
            _cardPool.Enqueue(card);
        }

        public void Close()
        {
            ResetAnimation();
        }
    }
}