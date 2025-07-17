using System;
using Sirenix.OdinInspector;
using Spire.Xls;
using UnityEngine;

namespace VTuber.BattleSystem.Effect
{
    public class VAttributeGainRateModifierEffectConfiguration : VEffectConfiguration
    {
        public string attributeName;
        public VAttributeGainRateModifierEffectConfiguration(CellRange row) : base(row)
        {
            attributeName = row.Columns[VEffectHeaderIndex.Parameter].Value;
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter) 
        {
            upgradable = parameter != upgradedParameter;
            return new VAttributeGainRateModifierEffect(this, parameter, upgradedParameter);
        }
    }
}