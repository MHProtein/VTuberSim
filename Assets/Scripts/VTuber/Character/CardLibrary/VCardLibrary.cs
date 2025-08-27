using System.Collections.Generic;
using VTuber.BattleSystem.Card;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;                                                                                                               

namespace VTuber.Character
{
    public class VCardLibrary
    {
        private List<VCard> cards;
        
        public VCardLibrary()
        {
            cards = new List<VCard>();
        }

        public void AddCards(List<VCard> cards)
        {
            foreach (var card in cards)
            {
                AddCard(card);
            }
        }
        
        public void AddCard(VCard card)
        {
            if (card != null)
            {
                cards.Add(card);
                VDebug.Log("Card added: " + card.CardName);
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnCardAdded, new Dictionary<string, object>()
                {
                    { "Card", card }
                });
            }
        }
        
        public void RemoveCard(VCard card)
        {
            if (card != null && cards.Contains(card))
            {
                cards.Remove(card);
                VDebug.Log("Card removed: " + card.CardName);
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnCardRemoved, new Dictionary<string, object>()
                {
                    { "Card", card }
                });
            }
        }
        
        public List<VCard> GetCards()
        {
            return cards;
        }

        public void ReplaceCard(VCard cardToReplace, VCard selectedCard)
        {
            if (cardToReplace != null && selectedCard != null)
            {
                int index = cards.IndexOf(selectedCard);
                cards[index] = cardToReplace;
                VDebug.Log("Card replaced: " + selectedCard.CardName + " with " + cardToReplace.CardName);
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnCardReplaced, new Dictionary<string, object>()
                {
                    { "CardToReplace", cardToReplace },
                    { "ReplacedCard", selectedCard }
                });
            }
        }

        public void Clear()
        {
            cards.Clear();
        }
    }
}