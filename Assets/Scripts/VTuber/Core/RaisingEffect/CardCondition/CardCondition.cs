using System.Collections.Generic;
using VTuber.BattleSystem.Card;

namespace VTuber.BattleSystem.Core.RaisingEffect.CardCondition
{
    public class CardCondition
    {
        public string Type;
        public VCardRarity Rarity;
		public List<uint> cardPool;
    }
}