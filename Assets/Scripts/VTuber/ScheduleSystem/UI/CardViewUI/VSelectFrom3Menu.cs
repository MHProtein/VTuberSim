using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.UI;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Dialogue.UI;

namespace VTuber.ScheduleSystem.UI
{
    public class VSelectFrom3CardsMenu : VUIBehaviour, ISelectableCardMenu
    {
        [SerializeField] private GameObject cardPrefab;
        private List<VSelectCardCardUI> _cardUIs;
        private VSelectCardCardUI _selectedCardUI;

        [SerializeField] private Button confirmButton;
        private Action<VCard> _confirmAction;

        public List<Transform> positions;
        public Transform spawnPosition;

        protected override void Awake()
        {
            base.Awake();
            confirmButton.onClick.AddListener(Confirm);
        }

        public void Initialize(List<VCard> cards, Action<VCard> confirmAction)
        {
            confirmButton.interactable = false;
            _confirmAction = confirmAction;
            int i = 0;
            _cardUIs = new List<VSelectCardCardUI>();
            foreach (var card in cards)
            {
                var item = Instantiate(cardPrefab, transform);
                var cardItem = item.AddComponent<VSelectCardCardUI>();
                var cardUI = cardItem.GetComponent<VCardUI>();
                cardUI.SetCard(card);
                
                cardItem.Initialize(cardUI, this, false);
                _cardUIs.Add(cardItem);
                
                cardUI.transform.localScale = Vector3.zero;
                cardUI.transform.position = spawnPosition.position;
                Tween.Position(cardUI.transform, positions[i].position, 0.5f);
                Tween.Scale(cardUI.transform, Vector3.one, 0.5f, Ease.OutBounce).OnComplete((() =>
                {
                    cardItem.SetSelectable(true);
                }));
                i++;
            }
        }

        public void Confirm()
        {
            _confirmAction?.Invoke(_selectedCardUI.Card);
            
            foreach (var cardUI in _cardUIs)
            {
                Destroy(cardUI.gameObject);
            }
            _cardUIs.Clear();
            _selectedCardUI = null;
            VEventSystemUI.Instance.CloseSelectFrom3Menu();
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