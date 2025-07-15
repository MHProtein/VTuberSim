using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.Effect
{
    public class VDrawCardEffect : VEffect
    {
        private int _drawCardCount;
        public VDrawCardEffect(VDrawCardEffectConfiguration configuration) : base(configuration)
        {
            _drawCardCount = configuration.drawCardCount;
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldPlayTwice = false)
        {
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnRequestDrawCards, new Dictionary<string, object>()
            {
                { "DrawCount", _drawCardCount },
                { "IsFromCard", isFromCard },
                { "ShouldPlayTwice", shouldPlayTwice }
            });
        }
    }
}