using System;
using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAbilityEffectConfiguration : VRaisingEffectConfiguration
    {
        public VRaisingAddAbilityEffectConfiguration(CellRange row) : base(row)
        {
            AbilityName = row.Columns[VRaisingEffectHeaderIndex.Param].Value;
            ShouldUseEfficiency = Convert.ToInt32(row.Columns[VRaisingEffectHeaderIndex.Condition].Value) == 1;
        }

        public string AbilityName { get; }

        public bool ShouldUseEfficiency { get; }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            var value = Convert.ToInt32(parameter);
            var upgradedValue = Convert.ToInt32(parameter);
            return new VRaisingAddAbilityEffect(this, value, upgradedValue);
        }
    }
}