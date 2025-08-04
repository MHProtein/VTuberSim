using System;
using Spire.Xls;
using VTuber.Core.RaisingEffect;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAttributeEffectConfiguration : VRaisingEffectConfiguration
    {
        public string AbilityName => _abilityName;
        private string _abilityName;
        public VRaisingAddAttributeEffectConfiguration(CellRange row) : base(row)
        {
            _abilityName = row.Columns[VRaisingEffectHeaderIndex.Param].Value;
        }

        public override VRaisingEffect CreateEffect(string parameter)
        {
            int value = Convert.ToInt32(parameter);
            return new VRaisingAddAttributeEffect(this, value);
        }
    }
}