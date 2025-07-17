using System;
using Spire.Xls;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Buff;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect
{
    public class VBuffModifyEffectConfiguration : VEffectConfiguration
    { 
        public uint buffID;

        public VBuffModifyEffectConfiguration(CellRange row) : base(row)
        {
            VDebug.Log(row.Columns[VEffectHeaderIndex.Parameter].Value);
            buffID = Convert.ToUInt32(row.Columns[VEffectHeaderIndex.Parameter].Value);
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            upgradable = parameter != upgradedParameter;
            return new VBuffModifyEffect(this, parameter, upgradedParameter);
        }
    }
}