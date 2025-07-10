using Sirenix.Utilities;

namespace VTuber.BattleSystem.Effect
{
    public class VDoublePlayEffectConfiguration : VEffectConfiguration
    {
        public override VEffect CreateEffect()
        {
            return new VDoublePlayEffect(this);
        }
    }
}