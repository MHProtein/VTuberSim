using CsvHelper;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Buff;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect
{
    public class VBuffModifyEffectConfiguration : VEffectConfiguration
    { 
        public int buffID;
            
        public int addValue;
        public float addPercentage;

        public VBuffModifyEffectConfiguration(CsvReader csv) : base(csv)
        {
            buffID = csv.GetField<int>("BuffID");
            addValue = csv.GetField<int>("BuffDelta");
        }

        public override VEffect CreateEffect()
        {
            return new VBuffModifyEffect(this);
        }
    }
}