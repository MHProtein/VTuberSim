using System.Collections.Generic;
using VTuber.BattleSystem.Card;

namespace VTuber.BattleSystem.Core.RaisingEffect
{
    public class VCardPoolCondition : VCardCondition
    {
        List<uint> _cardIds;
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