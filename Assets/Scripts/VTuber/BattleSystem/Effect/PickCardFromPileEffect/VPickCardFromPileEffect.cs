using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.Effect
{
    public class VPickCardFromPileEffect : VEffect
    {
        private VCardPileType _cardPileType;
        private VUpgradableValue<int> _cardCount;
        public VPickCardFromPileEffect(VPickCardFromPileEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _cardPileType = configuration.cardPileType;
            _cardCount = new VUpgradableValue<int>(Convert.ToInt32(parameter), Convert.ToInt32(upgradedParameter));
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnRequestPickCardsFromPile, new Dictionary<string, object>()
            {
                { "CardPileType", _cardPileType },
                { "CardCount", _cardCount.Value },
                { "IsFromCard", isFromCard },
                { "ShouldPlayTwice", shouldApplyTwice }
            });
        }
        
        public override void Upgrade()
        {
            base.Upgrade();
            _cardCount.Upgrade();
        }
        
        public override void Downgrade()
        {
            base.Downgrade();
            _cardCount.Downgrade();
        }
        
    }
}