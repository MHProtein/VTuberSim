using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VAttributeModifyCondition : VEffectCondition
    {
        private int _targetValue;
        private int _targetDelta;

        public VAttributeModifyCondition(CellRange row) : base(row)
        {
            _targetValue = ToInt( row.Columns[VConditionHeaderIndex.TargetValue].Value);
            _targetDelta = ToInt( row.Columns[VConditionHeaderIndex.TargetDelta].Value);
        }
 
        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            return Compare((int)message["NewValue"], _targetValue) && Compare((int)message["Delta"], _targetDelta);
        }
    }
}