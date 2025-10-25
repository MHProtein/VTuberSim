namespace VTuber.BattleSystem.Effect
{
    public abstract class VModifierEffect : VEffect
    {
        protected VModifierEffect(VEffectConfiguration configuration) : base(configuration)
        {
        }

        public abstract VModifierEffectSaveData Save();
        public abstract void Load(VModifierEffectSaveData data);
    }
}