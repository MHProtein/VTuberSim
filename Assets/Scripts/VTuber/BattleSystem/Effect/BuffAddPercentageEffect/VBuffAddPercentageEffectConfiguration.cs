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
            float percentage = Convert.ToSingle(parameter);
            float upgradedPercentage = Convert.ToSingle(upgradedParameter);
            return new VBuffAddPercentageEffect(this, buffID, percentage, upgradedPercentage);
        }
    }
}