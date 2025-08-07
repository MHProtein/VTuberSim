using System;
using Spire.Xls;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingReplaceCardEffectConfiguration : VRaisingEffectConfiguration
    {
        public VCardCondition Condition => _condition;
        private VCardCondition _condition;
        public VRaisingReplaceCardEffectConfiguration(CellRange row) : base(row)
        {
            
            _condition =
                VResourcesManager.Instance.GetCardConditionByID(
                    Convert.ToUInt32(row.Columns[VRaisingEffectHeaderIndex.Condition].Value));
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingReplaceCardEffect(this);
        }
    }
}