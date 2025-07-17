using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect
{
    public class VDoublePlayEffect : VEffect
    {
        public VDoublePlayEffect(VDoublePlayEffectConfiguration configuration) : base(configuration)
        {
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            battle.NextCardPlayTwice();
            VDebug.Log($"Effect {_configuration.effectName} applied: Next card will be played twice.");
        }
    }
}