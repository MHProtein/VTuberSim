using System;
using Spire.Xls;

namespace VTuber.BattleSystem.Effect.ShieldStaminaModifierEffect
{
    public class VShieldStaminaModifierEffectConfiguration : VEffectConfiguration
    {
        public VStaminaModifiyType modifyType;
        public VShieldStaminaModifierEffectConfiguration(CellRange row) : base(row)
        {
            modifyType = Enum.Parse<VStaminaModifiyType>(row.Columns[VEffectHeaderIndex.Parameter].Value);
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            upgradable = (parameter != upgradedParameter);
            
            return new VShieldStaminaModifierEffect(this, parameter, upgradedParameter);
        }
    }
}