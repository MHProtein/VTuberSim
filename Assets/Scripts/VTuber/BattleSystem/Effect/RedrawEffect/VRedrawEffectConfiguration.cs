using Spire.Xls;

namespace VTuber.BattleSystem.Effect
{
    public class VRedrawEffectConfiguration : VEffectConfiguration
    {
        public VRedrawEffectConfiguration(CellRange row) : base(row)
        {
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRedrawEffect(this);
        }
    }
}