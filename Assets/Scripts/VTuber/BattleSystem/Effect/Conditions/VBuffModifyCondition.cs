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
                VDebug.Log("条件 " + id + " 未通过：消息中缺少必要的Key。");
                return false;
            }

            if ((int)message["BuffId"] != _buffId)
            {
                VDebug.Log($"条件 {id} 未通过：Buff ID 不匹配。期望 {_buffId}，实际 {(int)message["BuffId"]}");
                return false;
            }

            bool result = Compare((int)message["Value"], _targetValue) && Compare((int)message["Delta"], _targetDelta);
            if (result)
            {
                VDebug.Log($"条件 {id} 通过：Buff(ID: {_buffId}) 的数值为 {(int)message["Value"]}，变化量为 {(int)message["Delta"]}");
            }
            else
            {
                VDebug.Log($"条件 {id} 未通过：Buff(ID: {_buffId}) 的数值为 {(int)message["Value"]}，变化量为 {(int)message["Delta"]}");
            }
            return result;
        }
    }
}