using System;
using Spire.Xls;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingReplaceCardEffectConfiguration : VRaisingCardEffectConfiguration
    {
        public VCardCondition Condition => _condition;
        private VCardCondition _condition;
        public VRaisingReplaceCardEffectConfiguration(CellRange row) : base(row)
        {
            string conditionStr = row.Columns[VRaisingEffectHeaderIndex.Condition].Value;
            if (string.IsNullOrEmpty(conditionStr))
                _condition = null;
            else
                _condition = VDataManager.Instance.GetCardConditionByID(uint.Parse(conditionStr.Trim()));
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingReplaceCardEffect(this);
        }
    }
}