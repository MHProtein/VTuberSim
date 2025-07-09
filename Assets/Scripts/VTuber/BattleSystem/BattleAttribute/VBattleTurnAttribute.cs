using System.Collections.Generic;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleTurnAttribute : VBattleAttribute
    {
        public VBattleTurnAttribute(int maxTurn) : base(maxTurn, false, VRootEventKey.OnTurnChange)
        {
        }
        
    }
}