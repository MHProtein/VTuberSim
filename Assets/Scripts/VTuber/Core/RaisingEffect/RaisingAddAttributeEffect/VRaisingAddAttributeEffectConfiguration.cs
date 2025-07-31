namespace VTuber.BattleSystem.Core.RaisingEffect.VRaisingAddAttributeEffect
{
    public class VRaisingAddAttributeEffectConfiguration : VRaisingEffectConfiguration
    {
        public override VRaisingEffect CreateEffect()
        {
            return new VRaisingAddAttributeEffect(this);
        }
    }
}