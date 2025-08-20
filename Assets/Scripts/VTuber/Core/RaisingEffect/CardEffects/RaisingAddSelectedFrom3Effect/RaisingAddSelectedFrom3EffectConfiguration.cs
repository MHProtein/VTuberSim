using System;
using Spire.Xls;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddSelectedFrom3EffectConfiguration : VRaisingCardEffectConfiguration
    {
        public VCardCondition Condition => _condition;
        private VCardCondition _condition;
        
        public VRaisingAddSelectedFrom3EffectConfiguration(CellRange row) : base(row)
        {
            string conditionStr = row.Columns[VRaisingEffectHeaderIndex.Condition].Value;
            if (string.IsNullOrEmpty(conditionStr))
                _condition = null;
            else
                _condition = VResourcesManager.Instance.GetCardConditionByID(uint.Parse(conditionStr.Trim()));
        }


        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingAddSelectedFrom3Effect(this);
        }
    }
}