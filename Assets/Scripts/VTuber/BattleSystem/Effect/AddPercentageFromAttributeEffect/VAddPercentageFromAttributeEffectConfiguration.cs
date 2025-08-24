using Spire.Xls;

namespace VTuber.BattleSystem.Effect
{
    public class VAddPercentageFromAttributeEffectConfiguration : VEffectConfiguration
    {
        public string attributeNameToAdd;
        public string attributeNameToBeAdded;

        public VAddPercentageFromAttributeEffectConfiguration(CellRange row, string attributeNameToAdd, string attributeNameToBeAdded) : base(row)
        {
            this.attributeNameToAdd = attributeNameToAdd;
            this.attributeNameToBeAdded = attributeNameToBeAdded;
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VAddPercentageFromAttributeEffect(this, parameter, upgradedParameter);
        }
    }
}