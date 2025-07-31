using VTuber.Character;

namespace VTuber.BattleSystem.Core.RaisingEffect
{
    public class VRaisingEffectConfiguration
    {
        public uint id;
        public string effectName;
        public string description;
        
        public virtual VRaisingEffect CreateEffect()
        {
            return new VRaisingEffect(this);
        }
        
    }
}