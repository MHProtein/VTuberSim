using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core
{
    public class VBattleConfiguration : VScriptableObject
    {
        public int maxTurnCount = 10;
        public int maxHandSize = 3;
        public int defaultPlayPerTurn = 1;
    }
}