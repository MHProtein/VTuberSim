using System;
using Spire.Xls;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddRandomCardEffectConfiguration : VRaisingEffectConfiguration
    {
        private readonly VCardCondition _condition;
        public VCardCondition Condition => _condition;
        public VRaisingAddRandomCardEffectConfiguration(CellRange row) : base(row)
        {
            _condition =
                VResourcesManager.Instance.GetCardConditionByID(
                    Convert.ToUInt32(row.Columns[VRaisingEffectHeaderIndex.Condition].Value));
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingAddRandomCardEffect(this);
        }
    }
}