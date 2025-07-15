using System;
using Sirenix.OdinInspector;
using Spire.Xls;
using UnityEngine;

namespace VTuber.BattleSystem.Effect
{
    public enum VStaminaCostModifierType
    {
        Rate,
        Points
    }
    public class VStaminaCostModiferEffectConfiguration : VEffectConfiguration
    {
        public VStaminaCostModifierType modifierType;
        
        public VStaminaCostModiferEffectConfiguration(CellRange row) : base(row)
        {
            modifierType = Enum.Parse<VStaminaCostModifierType>(row.Columns[VEffectHeaderIndex.Parameter].Value);
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter) 
        {
            return new VStaminaCostModiferEffect(this, parameter, upgradedParameter);
        }
    }
}