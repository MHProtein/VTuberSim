using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattlePlayLeftAttribute : VBattleAttribute
    {
        private int _defaultPlayCountPerTurn;
        public VBattlePlayLeftAttribute(int value) : base(value, false, VBattleEventKey.OnPlayLeftChange)
        {
            _defaultPlayCountPerTurn = value;
        }
        public VBattlePlayLeftAttribute(VBattleAttributeSaveData saveData) : base(saveData)
        {
            _defaultPlayCountPerTurn = saveData.defaultPlayCountPerTurn;
        }

        public override VBattleAttributeSaveData Save()
        {
            var data = base.Save();
            data.defaultPlayCountPerTurn = _defaultPlayCountPerTurn;
            return data;
        }
        
        public override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnBegin, dict => SetValue(_defaultPlayCountPerTurn, false));
        }
        
        public override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnBegin, dict => SetValue(_defaultPlayCountPerTurn, false));
        }
    }
}