namespace VTuber.BattleSystem.Effect
{
    public class VRedrawEffectConfiguration : VEffectConfiguration
    {
        public override VEffect CreateEffect()
        {
            return new VRedrawEffect(this);
        }
    }
}