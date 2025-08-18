using Spire.Xls;

namespace VTuber.BattleSystem.Effect
{
    public class VAddParamAttributePercentageEffectConfiguration : VEffectConfiguration
    {
        public string attributeName;
        public VAddParamAttributePercentageEffectConfiguration(CellRange row) : base(row)
        {
            attributeName = row.Columns[VEffectHeaderIndex.Parameter].Value;
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VAddParamAttributePercentageEffect(this, parameter, upgradedParameter);
        }
    }
}