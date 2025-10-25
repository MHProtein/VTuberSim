using Spire.Xls;

namespace VTuber.BattleSystem.Effect
{
    public class VDrawCardEffectConfiguration : VEffectConfiguration
    {
        public VDrawCardEffectConfiguration(CellRange row) : base(row)
        {
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            upgradable = parameter != upgradedParameter;
            return new VDrawCardEffect(this, parameter, upgradedParameter);
        }
    }
}