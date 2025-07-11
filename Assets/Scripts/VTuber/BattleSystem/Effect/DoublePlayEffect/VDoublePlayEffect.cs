using VTuber.BattleSystem.Core;

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
        }
    }
}