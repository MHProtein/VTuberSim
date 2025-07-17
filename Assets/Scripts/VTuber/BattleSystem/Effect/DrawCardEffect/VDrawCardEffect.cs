using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect
{
    public class VDrawCardEffect : VEffect
    {
        private VUpgradableValue<int> _drawCardCount;
        public VDrawCardEffect(VDrawCardEffectConfiguration configuration,string parameter, string upgradedParameter) : base(configuration)
        {
            _drawCardCount = new VUpgradableValue<int>(Convert.ToInt32(parameter), Convert.ToInt32(upgradedParameter));
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldPlayTwice = false)
        {
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRequestDrawCards, new Dictionary<string, object>()
            {
                { "DrawCount", _drawCardCount.Value },
                { "IsFromCard", isFromCard },
                { "ShouldPlayTwice", shouldPlayTwice }
            });
            VDebug.Log($"Effect {_configuration.effectName} requested to draw {_drawCardCount.Value} cards.");
        }        
        
        public override void Upgrade()
        {
            base.Upgrade();
            _drawCardCount.Upgrade();
        }
        
        public override void Downgrade()
        {
            base.Downgrade();
            _drawCardCount.Downgrade();
        }
    }
}