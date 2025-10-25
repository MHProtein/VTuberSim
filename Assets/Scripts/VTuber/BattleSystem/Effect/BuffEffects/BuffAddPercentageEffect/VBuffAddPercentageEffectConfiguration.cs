using System;
using Spire.Xls;

namespace VTuber.BattleSystem.Effect
{
    public class VBuffAddPercentageEffectConfiguration : VEffectConfiguration
    {
        public uint buffID;

        public VBuffAddPercentageEffectConfiguration(CellRange row) : base(row)
        {
            buffID = Convert.ToUInt32(row.Columns[VEffectHeaderIndex.Parameter].Value);
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            var percentage = Convert.ToSingle(parameter);
            var upgradedPercentage = Convert.ToSingle(upgradedParameter);
            return new VBuffAddPercentageEffect(this, buffID, percentage, upgradedPercentage);
        }
    }
}