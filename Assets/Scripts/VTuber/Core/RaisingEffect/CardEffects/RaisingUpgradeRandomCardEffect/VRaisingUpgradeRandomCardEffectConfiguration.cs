using Spire.Xls;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeRandomCardEffectConfiguration : VRaisingCardEffectConfiguration
    {
        private readonly VCardCondition _condition;
        public VCardCondition Condition => _condition;
        public VRaisingUpgradeRandomCardEffectConfiguration(CellRange row) : base(row)
        {
            string conditionStr = row.Columns[VRaisingEffectHeaderIndex.Condition].Value;
            if (string.IsNullOrEmpty(conditionStr))
                _condition = null;
            else
                _condition = VResourcesManager.Instance.GetCardConditionByID(uint.Parse(conditionStr.Trim()));
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingUpgradeRandomCardEffect(this);
        }
    }
}