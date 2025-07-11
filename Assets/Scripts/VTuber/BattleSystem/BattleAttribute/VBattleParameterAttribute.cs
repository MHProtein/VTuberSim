using System.Collections.Generic;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleParameterAttribute : VBattleAttribute
    {
        public VBattleParameterAttribute(int value) : base(value, false, VRootEventKey.OnParameterChange)
        {
            
        }

        public override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
        }

        void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            SetValue(0, false);
        }
        
    }
}