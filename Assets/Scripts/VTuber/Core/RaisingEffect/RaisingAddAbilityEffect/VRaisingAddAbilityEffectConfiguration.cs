using System;
using Spire.Xls;
using VTuber.Core.RaisingEffect;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAbilityEffectConfiguration : VRaisingEffectConfiguration
    {
        public string AbilityName => _abilityName;
        private string _abilityName;

        public bool ShouldUseEfficiency => _shouldUseEfficiency;
        private bool _shouldUseEfficiency;
        
        public VRaisingAddAbilityEffectConfiguration(CellRange row) : base(row)
        {
            _abilityName = row.Columns[VRaisingEffectHeaderIndex.Param].Value;
            _shouldUseEfficiency = Convert.ToInt32(row.Columns[VRaisingEffectHeaderIndex.Condition].Value) == 1;
        }

        public override VRaisingEffect CreateEffect(string parameter)
        {
            int value = Convert.ToInt32(parameter);
            return new VRaisingAddAbilityEffect(this, value);
        }
    }
}