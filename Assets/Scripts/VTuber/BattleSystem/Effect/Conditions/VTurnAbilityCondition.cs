using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VTurnAbilityCondition : VEffectCondition
    {
        private string _attributeName;
        public VTurnAbilityCondition(CellRange row) : base(row)
        {
            _attributeName = row.Columns[VConditionHeaderIndex.TargetValue].Value.Trim();
        }

        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            return battle.BattleAttributeManager.MultiplierManager.Multiplier.AttributeName == _attributeName;
        }
    }
}