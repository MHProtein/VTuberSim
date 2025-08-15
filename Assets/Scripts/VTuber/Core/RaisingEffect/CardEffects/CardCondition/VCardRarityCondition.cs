using System;
using Spire.Xls;
using VTuber.BattleSystem.Card;

namespace VTuber.Core.RaisingEffect
{
    public class VCardRarityCondition : VCardCondition
    {
        private VCardRarity _cardRarity;

        public VCardRarityCondition(CellRange row) : base(row)
        {
            _cardRarity = Enum.Parse<VCardRarity>(row.Columns[VCardConditionHeaderIndex.Condition].Value);
        }

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