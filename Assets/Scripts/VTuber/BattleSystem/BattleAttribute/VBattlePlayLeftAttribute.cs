using System.Collections.Generic;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattlePlayLeftAttribute : VBattleAttribute
    {
        private int _defaultPlayCountPerTurn;
        public VBattlePlayLeftAttribute(int value) : base(value, false, VRootEventKey.OnPlayLeftChange)
        {
            _defaultPlayCountPerTurn = value;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnBegin, dict => SetValue(_defaultPlayCountPerTurn));
        }
        
        public override void OnDisable()
        {
            base.OnDisable();
            VRootEventCenter.Instance.RemoveListener(VRootEventKey.OnTurnBegin, dict => SetValue(_defaultPlayCountPerTurn));
        }
    }
}