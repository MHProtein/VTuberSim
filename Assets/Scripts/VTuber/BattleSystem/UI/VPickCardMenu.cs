using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VPickCardMenu : VUIBehaviour
    {
        [SerializeField] private TMP_Text SelectCardText;
        [SerializeField] private Button ConfirmButton;
        private List<VCard> _pickedCards;
        private List<VCardUI> _cardUIs;
        private int _maxPickCount = 3;
        private VCardPileType _cardPileType;
        
        private bool _isFromCard;
        private bool _shouldPlayTwice;
        
        public void BeginPickCard(List<VCardUI> cardsToSpawn, int maxPickCount, VCardPileType cardPileType,  bool isFromCard, bool shouldPlayTwice)
        {
            ConfirmButton.interactable = true;
            _maxPickCount = maxPickCount;
            _cardPileType = cardPileType;
            _pickedCards = new List<VCard>();
            SelectCardText.text = "Remaining picks: " + maxPickCount;
            
            foreach (var card in cardsToSpawn)
            {
                VPickCardUI pickCardUI = card.AddComponent<VPickCardUI>();
                pickCardUI.Initialize(card, this);
            }
            _cardUIs = cardsToSpawn;
            _isFromCard = isFromCard;
            _shouldPlayTwice = shouldPlayTwice;
        }
        
        public bool SelectCard(VCard pickCard)
        {
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

            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnCardsPickedFromPile, new Dictionary<string, object>()
            {
                { "CardPileType", _cardPileType },
                { "PickedCards", new List<VCard>(_pickedCards) }, 
                { "IsFromCard", _isFromCard }, 
                { "ShouldPlayTwice", _shouldPlayTwice }
            });

            foreach (var cardUI in _cardUIs)
            {
                Destroy(cardUI.gameObject);
            }
            _cardUIs.Clear();
            _pickedCards.Clear();
            
        }
    }
}