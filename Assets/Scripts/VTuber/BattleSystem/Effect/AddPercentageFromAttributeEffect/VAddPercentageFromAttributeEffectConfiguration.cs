using Spire.Xls;

namespace VTuber.BattleSystem.Effect
{
    public class VAddPercentageFromAttributeEffectConfiguration : VEffectConfiguration
    {
        public string attributeNameToAdd;
        public string attributeNameToBeAdded;

        public VAddPercentageFromAttributeEffectConfiguration(CellRange row) : base(row)
        {
            var parameters = row.Columns[VEffectHeaderIndex.Parameter].Value.Split(',');
            attributeNameToAdd = parameters[0];
            attributeNameToBeAdded = parameters[1];
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VAddPercentageFromAttributeEffect(this, parameter, upgradedParameter);
        }
    }
}