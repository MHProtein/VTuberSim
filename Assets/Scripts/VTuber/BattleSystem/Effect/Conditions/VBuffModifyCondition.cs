using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VBuffModifyCondition : VEffectCondition
    {
        private int _buffId;
        private int _targetValue;
        private int _targetDelta;


        public VBuffModifyCondition(CellRange row) : base(row)
        {
            _buffId = ToInt(row.Columns[VConditionHeaderIndex.NameOrID].Value);
            _targetValue = ToInt( row.Columns[VConditionHeaderIndex.TargetValue].Value);
            _targetDelta = ToInt( row.Columns[VConditionHeaderIndex.TargetDelta].Value);
        }

        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            if ((int)message["BuffId"] != _buffId)
                return false;

            return Compare((int)message["Value"], _targetValue) && Compare((int)message["Delta"], _targetDelta);
        }
    }
}