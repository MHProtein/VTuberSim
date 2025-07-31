namespace VTuber.BattleSystem.Core.RaisingEffect.RaisingAddRandomCardEffect
{
    public class VRaisingAddRandomCardEffectConfiguration : VRaisingEffectConfiguration
    {
        public override VRaisingEffect CreateEffect()
        {
            return new VRaisingAddRandomCardEffect(this);
        }
    }
}