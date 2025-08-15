using Spire.Xls;
using VTuber.BattleSystem.Card;

namespace VTuber.Core.RaisingEffect
{
    public class VCardTagCondition : VCardCondition
    {
        private string _tag;
        public VCardTagCondition(CellRange row) : base(row)
        {
            _tag = row.Columns[VCardConditionHeaderIndex.Condition].Value.Trim();
        }

        public override bool IsTrue(VCard card)
        {
            return card.Tags.Contains(_tag);
        }

        public override bool IsTrue(VCardConfiguration cardConfig)
        {
            return cardConfig.tags.Contains(_tag);
        }
    }
}