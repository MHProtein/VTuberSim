using Spire.Xls;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect
{
    public class VAddEffectConfiguration : VEffectConfiguration
    {
        [StringToEnum("BattleAttributes")] public string attributeName;

        public VAddEffectConfiguration(CellRange row) : base(row)
        {
            attributeName = row.Columns[VEffectHeaderIndex.Parameter].Value;
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VAddEffect(this, parameter, upgradedParameter);
        }
    }
}