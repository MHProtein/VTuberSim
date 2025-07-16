using System.Collections.Generic;
using VTuber.BattleSystem.Card;

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
            }
        }
        
        public void RemoveCard(VCard card)
        {
            if (card != null && cards.Contains(card))
            {
                cards.Remove(card);
            }
        }
        
        public List<VCard> GetCards()
        {
            return cards;
        }
        
    }
}