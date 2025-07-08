using System.Collections.Generic;
using VTuber.BattleSystem.Card;

namespace VTuber.Character
{
    public class VCardLibrary
    {
        private List<VCardConfiguration> cards;
        
        public VCardLibrary()
        {
            cards = new List<VCardConfiguration>();
        }
        
        public void AddCard(VCardConfiguration card)
        {
            if (card != null)
            {
                cards.Add(card);
            }
        }
        
        public void RemoveCard(VCardConfiguration card)
        {
            if (card != null && cards.Contains(card))
            {
                cards.Remove(card);
            }
        }
        
        public List<VCard> GetCards()
        {
            List<VCard> cardList = new List<VCard>();
            foreach (var cardConfig in cards)
            {
                if (cardConfig != null)
                {
                    cardList.Add(cardConfig.CreateCard());
                }
            }
            return cardList;
        }
        
    }
}