using System.Collections.Generic;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleParameterAttribute : VBattleAttribute
    {
        public VBattleParameterAttribute(int value) : base(value, false)
        {
            
        }

        public override void OnEnable()
        {
            base.OnEnable();
            VRootEventCenter.Instance.RegisterListener(VRootEventKeys.OnTurnEnd, OnTurnEnd);

            void OnTurnEnd(Dictionary<string, object> messagedict)
            {
                Value = 0; 
            }
        }
    }
}