using Spire.Xls;
using VTuber.BattleSystem.Card;

namespace VTuber.Core.RaisingEffect
{
    public class VCardTypeCondition : VCardCondition
    {
        string _cardType;

        public VCardTypeCondition(CellRange row) : base(row)
        {
            _cardType = row.Columns[VCardConditionHeaderIndex.Condition].Value;
        }

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