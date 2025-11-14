using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.Foundation;
using VTuber.Core.SE;

namespace VTuber.BattleSystem.UI
{
    public class VPickCardMenu : VUIBehaviour
    {
        [SerializeField] private TMP_Text SelectCardText;
        [SerializeField] private Button ConfirmButton;
        private VCardPileType _cardPileType;
        private List<VCardUI> _cardUIs;

        private bool _isFromCard;
        private int _maxPickCount = 3;
        private List<VCard> _pickedCards;
        private bool _shouldPlayTwice;

        private Action<List<VCard>> _onCardPicked;

        protected override void Awake()
        {
            base.Awake();
            ConfirmButton.onClick.AddListener(ConfirmSelection);
        }

        public void BeginPickCard(List<VCardUI> cardsToSpawn, int maxPickCount, VCardPileType cardPileType,
            bool isFromCard, bool shouldPlayTwice, Action<List<VCard>> onCardPicked)
        {
            _onCardPicked = onCardPicked;
            ConfirmButton.interactable = true;
            _maxPickCount = maxPickCount;
            _cardPileType = cardPileType;
            _pickedCards = new List<VCard>();
            SelectCardText.text = "Remaining picks: " + maxPickCount;

            foreach (var card in cardsToSpawn)
            {
                var pickCardUI = card.gameObject.AddComponent<VPickCardUI>();
                pickCardUI.Initialize(card, this);
            }

            _cardUIs = cardsToSpawn;
            _isFromCard = isFromCard;
            _shouldPlayTwice = shouldPlayTwice;
        }

        public bool SelectCard(VCard pickCard)
        {
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
            if (_pickedCards.Count >= _maxPickCount)
                return false;

            if (pickCard != null) _pickedCards.Add(pickCard);
            SelectCardText.text = "Remaining picks: " + (_maxPickCount - _pickedCards.Count);

            return true;
        }

        public void RemoveCard(VCard pickCard)
        {
            if (_pickedCards.Contains(pickCard))
            {
                _pickedCards.Remove(pickCard);
                SelectCardText.text = "Remaining picks: " + (_maxPickCount - _pickedCards.Count);
            }
        }

        public void ConfirmSelection()
        {
            SelectCardText.text = $"Selected {_pickedCards.Count} cards.";
            ConfirmButton.interactable = false;

            if (_onCardPicked is not null)
            {
                _onCardPicked?.Invoke(_pickedCards);
            }
            else
            {           
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardsPickedFromPile,
                new Dictionary<string, object>
                {
                    { "CardPileType", _cardPileType },
                    { "PickedCards", new List<VCard>(_pickedCards) },
                    { "IsFromCard", _isFromCard },
                    { "ShouldPlayTwice", _shouldPlayTwice }
                });
            }

            foreach (var cardUI in _cardUIs) Destroy(cardUI.gameObject);
            _cardUIs.Clear();
            _pickedCards.Clear();
        }
    }
}