namespace VTuber.BattleSystem.Core.RaisingEffect.RaisingAddAbilityEffect
{
    public class VRaisingAddAbilityEffectConfiguration : VRaisingEffectConfiguration
    {
        public override VRaisingEffect CreateEffect()
        {
            return new VRaisingAddAbilityEffect(this);
        }
    }
}