using VTuber.BattleSystem.Card;

namespace VTuber.BattleSystem.Core.RaisingEffect
{
    public class VCardRarityCondition : VCardCondition
    {
        private VCardRarity _cardRarity;

        public override bool IsTrue(VCard card)
        {
            return card.Rarity == _cardRarity;
        }
        
        public override bool IsTrue(VCardConfiguration cardConfig)
        {
            return cardConfig.rarity == _cardRarity;
        }
    }
}