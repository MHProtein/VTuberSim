using System;
using System.Collections.Generic;
using System.Linq;
using Spire.Xls;
using VTuber.BattleSystem.Card;

namespace VTuber.Core.RaisingEffect
{
    public class VCardPoolCondition : VCardCondition
    {
        private readonly List<uint> _cardIds;

        public VCardPoolCondition(CellRange row) : base(row)
        {
            var str = row.Columns[VCardConditionHeaderIndex.Condition].Value;
            _cardIds = str.Split(',').Select(cardId => Convert.ToUInt32(cardId)).ToList();
        }

        public override bool IsTrue(VCard card)
        {
            return _cardIds.Contains(card.configID);
        }

        public override bool IsTrue(VCardConfiguration cardConfig)
        {
            return _cardIds.Contains(cardConfig.id);
        }
    }
}