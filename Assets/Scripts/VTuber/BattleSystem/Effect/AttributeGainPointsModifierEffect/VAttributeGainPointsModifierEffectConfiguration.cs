using System;
using Sirenix.OdinInspector;
using Spire.Xls;
using UnityEngine;

namespace VTuber.BattleSystem.Effect
{

    public class VAttributeGainPointsModifierEffectConfiguration : VEffectConfiguration
    {
        public string attributeName;
        public VAttributeGainPointsModifierEffectConfiguration(CellRange row) : base(row)
        {
            attributeName = row.Columns[VEffectHeaderIndex.Parameter].Value;
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter) 
        {
            upgradable = parameter != upgradedParameter;
            return new VAttributeGainPointsModifierEffect(this, parameter, upgradedParameter);
        }
    }
}