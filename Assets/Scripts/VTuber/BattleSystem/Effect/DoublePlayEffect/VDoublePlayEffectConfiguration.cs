using CsvHelper;
using Sirenix.Utilities;

namespace VTuber.BattleSystem.Effect
{
    public class VDoublePlayEffectConfiguration : VEffectConfiguration
    {
        public VDoublePlayEffectConfiguration(CsvReader csv) : base(csv)
        {
        }

        public override VEffect CreateEffect()
        {
            return new VDoublePlayEffect(this);
        }
    }
}