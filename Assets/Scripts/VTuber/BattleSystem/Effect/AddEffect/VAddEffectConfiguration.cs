using UnityEngine.Serialization;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect
{
    public class VAddEffectConfiguration : VEffectConfiguration
    {
        [StringToEnum("BattleAttributes")] public string attributeName;
        public int addValue;
        public bool multiplyByLayer = false;

        public override VEffect CreateEffect()
        {
            return new VAddEffect(this);
        }
    }
}