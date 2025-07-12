using UnityEngine.Serialization;
using VTuber.BattleSystem.Buff;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect
{
    public class VBuffModifyEffectConfiguration : VEffectConfiguration
    { 
        public VBuffConfiguration buffConfig;
        
        public int addValue;
        
        public override VEffect CreateEffect()
        {
            return new VBuffModifyEffect(this);
        }
    }
}