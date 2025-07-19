using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleTurnAttribute : VBattleAttribute
    {
        public int MaxTurn { get; private set; }
        public VBattleTurnAttribute(int maxTurn) : base(maxTurn, false, VBattleEventKey.OnTurnChange)
        {
            MaxTurn = maxTurn;
        }
        
    }
}