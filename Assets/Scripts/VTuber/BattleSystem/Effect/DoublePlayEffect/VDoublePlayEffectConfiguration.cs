using Sirenix.Utilities;
using Spire.Xls;

namespace VTuber.BattleSystem.Effect
{
    public class VDoublePlayEffectConfiguration : VEffectConfiguration
    {
        public VDoublePlayEffectConfiguration(CellRange row) : base(row)
        {
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            upgradable = false;
            return new VDoublePlayEffect(this);
        }
    }
}