namespace VTuber.BattleSystem.Core.RaisingEffect
{
    public class VRaisingReplaceCardEffectConfiguration : VRaisingEffectConfiguration
    {
        public override VRaisingEffect CreateEffect()
        {
            return new VRaisingReplaceCardEffect(this);
        }
    }
}