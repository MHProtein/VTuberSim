using CsvHelper;
using UnityEngine.Serialization;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect
{
    public class VAddEffectConfiguration : VEffectConfiguration
    {
        [StringToEnum("BattleAttributes")] public string attributeName;
        public int addValue;

        public VAddEffectConfiguration(CsvReader csv) : base(csv)
        {
            attributeName = csv.GetField<string>("AttributeName");
            addValue = csv.GetField<int>("AttributeDelta");
        }

        public override VEffect CreateEffect()
        {
            return new VAddEffect(this);
        }
    }
}