using System.Collections.Generic;
using VTuber.BattleSystem.Card;

namespace VTuber.BattleSystem.Core.RaisingEffect
{
    public abstract class VCardCondition
    {
        public string Type;
        public VCardRarity Rarity;
		public List<uint> cardPool;

        public abstract bool IsTrue(VCard card);
        public abstract bool IsTrue(VCardConfiguration cardConfig);
    }
}