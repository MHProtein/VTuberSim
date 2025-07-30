using VTuber.BattleSystem.Card;

namespace VTuber.BattleSystem.Core.RaisingEffect
{
    public class VCardTypeCondition : VCardCondition
    {
        string _cardType;
        
        public override bool IsTrue(VCard card)
        {
            return card.CardType == _cardType;
        }

        public override bool IsTrue(VCardConfiguration cardConfig)
        {
            return cardConfig.cardType == _cardType;
        }
    }
}