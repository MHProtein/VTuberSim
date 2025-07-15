using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.Effect
{
    public class VPickCardFromPileEffect : VEffect
    {
        public VCardPileType cardPileType;
        public int cardCount;
        public VPickCardFromPileEffect(VPickCardFromPileEffectConfiguration configuration) : base(configuration)
        {
            cardPileType = configuration.cardPileType;
            cardCount = configuration.cardCount;
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnRequestPickCardsFromPile, new Dictionary<string, object>()
            {
                { "CardPileType", cardPileType },
                { "CardCount", cardCount },
                { "IsFromCard", isFromCard },
                { "ShouldPlayTwice", shouldApplyTwice }
            });
        }
    }
}