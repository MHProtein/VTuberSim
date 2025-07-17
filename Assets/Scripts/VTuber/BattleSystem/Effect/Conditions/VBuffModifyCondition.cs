using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

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
            if (!message.ContainsKey("NewValue") || !message.ContainsKey("Delta") || !message.ContainsKey("BuffId"))
            {
                VDebug.Log("Condition " + id + " failed: Required keys not found in message.");
                return false;
            }

            if ((int)message["BuffId"] != _buffId)
            {
                VDebug.Log($"Condition {id} failed: Buff ID mismatch. Expected {_buffId}, got {(int)message["BuffId"]}");
                return false;
            }

            bool result = Compare((int)message["Value"], _targetValue) && Compare((int)message["Delta"], _targetDelta);
            if (result)
            {
                VDebug.Log($"Condition {id} passed: Buff with ID {_buffId} has value {(int)message["Value"]} and delta {(int)message["Delta"]}");
            }
            else
            {
                VDebug.Log($"Condition {id} failed: Buff with ID {_buffId} has value {(int)message["Value"]} and delta {(int)message["Delta"]}");
            }
            return result;
        }
    }
}