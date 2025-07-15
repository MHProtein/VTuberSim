using System.Collections.Generic;
using Unity.VisualScripting;
using VTuber.BattleSystem.Card;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VPickCardMenu : VUIBehaviour
    {
        private List<VCard> pickedCards;
        
        public void BeginPickCard(List<VCardUI> cardsToSpawn)
        {
            pickedCards = new List<VCard>();
            foreach (var card in cardsToSpawn)
            {
                VPickCardUI pickCardUI = card.AddComponent<VPickCardUI>();
                pickCardUI.Initialize(card.Card, PickCard);
            }
        }
        
        public void PickCard(VCard pickCard)
        {
            if (pickCard != null) pickedCards.Add(pickCard);
        }
    }
}