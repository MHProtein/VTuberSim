namespace VTuber.BattleSystem.Buff
{
    public class VAdditionBuffConfiguration : VBuffConfiguration
    {
        public int addAmount;

        protected override void Awake()
        {
            base.Awake();
            buffType = typeof(VAdditionBuff);
        }
    }
}