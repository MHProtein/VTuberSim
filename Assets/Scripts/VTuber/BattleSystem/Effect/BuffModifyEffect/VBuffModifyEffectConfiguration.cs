using UnityEngine.Serialization;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect
{
    public class VBuffModifyEffectConfiguration : VEffectConfiguration
    {
        [StringToEnum("Buffs")] public string buffName;

        public int addValue;
        
        public override VEffect CreateEffect()
        {
            return new VBuffModifyEffect(this);
        }
    }
}