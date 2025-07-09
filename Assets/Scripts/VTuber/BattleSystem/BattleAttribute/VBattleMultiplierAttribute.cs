using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleMultiplierAttribute : VBattleAttribute
    {
        public VBattleMultiplierAttribute(int value) : base(value, true, VRootEventKey.OnMultiplierChange)
        {
            
        }
    }
}