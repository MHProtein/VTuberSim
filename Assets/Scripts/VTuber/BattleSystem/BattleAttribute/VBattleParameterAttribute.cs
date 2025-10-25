using System.Collections.Generic;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleParameterAttribute : VBattleAttribute
    {
        public VBattleParameterAttribute(int value) : base(value, false, VBattleEventKey.OnParameterChange)
        {
            gainPointsModifier.SetEventKey(VBattleEventKey.OnParameterPopularityModifierChanged);
            gainRateModifier.SetEventKey(VBattleEventKey.OnParameterPopularityModifierChanged);
        }

        public VBattleParameterAttribute(VBattleAttributeSaveData saveData) : base(saveData)
        {
            gainPointsModifier.SetEventKey(VBattleEventKey.OnParameterPopularityModifierChanged);
            gainRateModifier.SetEventKey(VBattleEventKey.OnParameterPopularityModifierChanged);
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

        private void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            SetValue(0, false);
        }
    }
}