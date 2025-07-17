using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

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
            if (!message.ContainsKey("NewValue") || !message.ContainsKey("Delta"))
            {
                VDebug.Log("Condition " + id + " failed: Required keys 'NewValue' or 'Delta' not found in message.");
                return false;
            }
            bool result = Compare((int)message["NewValue"], _targetValue) && Compare((int)message["Delta"], _targetDelta);
            if (result)
            {
                VDebug.Log($"Condition {id} passed: Attribute has new value {(int)message["NewValue"]} and delta {(int)message["Delta"]}");
            }
            else
            {
                VDebug.Log($"Condition {id} failed: Attribute has new value {(int)message["NewValue"]} and delta {(int)message["Delta"]}");
            }
            return result;
        }
    }
}