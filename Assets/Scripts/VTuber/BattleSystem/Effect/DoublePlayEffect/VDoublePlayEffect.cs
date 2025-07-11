using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect
{
    public class VDoublePlayEffect : VEffect
    {
        public VDoublePlayEffect(VDoublePlayEffectConfiguration configuration) : base(configuration)
        {
        }

        public override void ApplyEffect(VBattle battle)
        {
            battle.NextCardPlayTwice();
        }
    }
}