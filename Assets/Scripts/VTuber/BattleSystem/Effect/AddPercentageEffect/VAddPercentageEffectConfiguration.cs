using Spire.Xls;

namespace VTuber.BattleSystem.Effect.AddPercentageEffect
{
    public class VAddPercentageEffectConfiguration : VEffectConfiguration 
    {
        
        public string attributeName;
        public VAddPercentageEffectConfiguration(CellRange row) : base(row)
        {
            attributeName = row.Columns[VEffectHeaderIndex.Parameter].Value;
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VAddPercentageEffect(this, parameter, upgradedParameter);
        }
    }
}