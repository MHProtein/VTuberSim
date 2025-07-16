using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleParameterAttribute : VBattleAttribute
    {
        public VBattleParameterAttribute(int value) : base(value, false, VBattleEventKey.OnParameterChange)
        {
            
        }

        public override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
        }

        void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            SetValue(0, false);
        }
        
    }
}