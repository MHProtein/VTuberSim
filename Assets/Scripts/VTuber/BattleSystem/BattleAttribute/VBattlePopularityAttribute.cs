using System.Collections.Generic;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattlePopularityAttribute : VBattleAttribute
    {
        public VBattlePopularityAttribute(int value) : base(value, false, VRootEventKey.OnPopularityChange)
        {
        }
    }
}