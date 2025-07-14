using CsvHelper;

namespace VTuber.BattleSystem.Effect
{
    public class VRedrawEffectConfiguration : VEffectConfiguration
    {
        public VRedrawEffectConfiguration(CsvReader csv) : base(csv)
        {
        }

        public override VEffect CreateEffect()
        {
            return new VRedrawEffect(this);
        }
    }
}