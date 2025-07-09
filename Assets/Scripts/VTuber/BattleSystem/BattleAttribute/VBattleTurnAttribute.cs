namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleTurnAttribute : VBattleAttribute
    {
        public VBattleTurnAttribute(int maxTurn) : base(maxTurn, false)
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