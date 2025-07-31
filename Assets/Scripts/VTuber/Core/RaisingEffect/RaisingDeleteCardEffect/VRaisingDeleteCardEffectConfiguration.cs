namespace VTuber.BattleSystem.Core.RaisingEffect
{
    public class VRaisingDeleteCardEffectConfiguration : VRaisingEffectConfiguration
    {
        public override VRaisingEffect CreateEffect()
        {
            return new VRaisingDeleteCardEffect(this);
        }
    }
}