using System;
using Spire.Xls;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAttributeEffectConfiguration : VRaisingEffectConfiguration
    {
        public VRaisingAddAttributeEffectConfiguration(CellRange row) : base(row)
        {
            AbilityName = row.Columns[VRaisingEffectHeaderIndex.Param].Value;
        }

        public string AbilityName { get; }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            var value = Convert.ToInt32(parameter.Trim());
            var upgradedValue = Convert.ToInt32(parameter.Trim());
            return new VRaisingAddAttributeEffect(this, value, upgradedValue);
        }
    }
}