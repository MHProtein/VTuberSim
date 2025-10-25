using System;
using Spire.Xls;

namespace VTuber.BattleSystem.Effect
{
    public class VBuffModifyEffectConfiguration : VEffectConfiguration
    {
        public uint buffID;

        public VBuffModifyEffectConfiguration(CellRange row) : base(row)
        {
            buffID = Convert.ToUInt32(row.Columns[VEffectHeaderIndex.Parameter].Value);
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            upgradable = parameter != upgradedParameter;
            return new VBuffModifyEffect(this, parameter, upgradedParameter);
        }
    }
}