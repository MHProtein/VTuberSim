using Spire.Xls;
using VTuber.BattleSystem.Card;

namespace VTuber.Core.RaisingEffect
{
    public class VCardLiveTypeCondition : VCardCondition
    {
        private string _liveType;
        public VCardLiveTypeCondition(CellRange row) : base(row)
        {
            _liveType = row.Columns[VCardConditionHeaderIndex.Condition].Value.Trim();
        }

        public override bool IsTrue(VCard card)
        {
            return card.LiveType == _liveType;
        }

        public override bool IsTrue(VCardConfiguration cardConfig)
        {
            return cardConfig.liveType == _liveType;
        }
    }
}