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
                VDebug.Log($"条件 {id} 未通过：消息中未找到 'NewValue' 或 'Delta' 键。");
                return false;
            }
            bool result = Compare((int)message["NewValue"], _targetValue) && Compare((int)message["Delta"], _targetDelta);
            if (result)
            {
                VDebug.Log($"条件 {id} 通过：属性新值为 {(int)message["NewValue"]}，变化量为 {(int)message["Delta"]}");
            }
            else
            {
                VDebug.Log($"条件 {id} 未通过：属性新值为 {(int)message["NewValue"]}，变化量为 {(int)message["Delta"]}");
            }
            return result;
        }
    }
}