namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleTurnAttribute : VBattleAttribute
    {
        public VBattleTurnAttribute(int maxTurn, bool isPercentage) : base(maxTurn, isPercentage)
        {
        }
        
        public void DecreaseTurn()
        {
            if (Value > 0)
            {
                --Value;
            }
        }
        
        
    }
}