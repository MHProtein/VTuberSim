using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VAttributeCondition : VEffectCondition
    {
        private string _attributeName;
        private int _targetValue;

        public VAttributeCondition(CellRange row) : base(row)
        {
            _attributeName = row.Columns[VConditionHeaderIndex.NameOrID].Value;
            _targetValue = ToInt( row.Columns[VConditionHeaderIndex.TargetValue].Value);
        }
        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            if (battle.BattleAttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                return Compare(attribute.Value, _targetValue);
            }

            VDebug.Log($"条件 {id} 未通过：战斗中未找到属性 {_attributeName}。");
            return false;
        }
    }
}