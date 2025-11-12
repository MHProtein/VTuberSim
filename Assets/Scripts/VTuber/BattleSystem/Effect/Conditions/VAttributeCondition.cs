using System;
using System.Collections.Generic;
using Spire.Xls;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.Effect.Conditions
{
    [Serializable]
    public class VAttributeCondition : VEffectCondition
    {
        [SerializeField] public string attributeName;
        [SerializeField] private int targetValue;

        public VAttributeCondition(CellRange row) : base(row)
        {
            attributeName = row.Columns[VConditionHeaderIndex.NameOrID].Value;
            targetValue = ToInt(row.Columns[VConditionHeaderIndex.TargetValue].Value);
        }

        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            if (battle.BattleAttributeManager.TryGetAttribute(attributeName, out var attribute))
                return VMathUtils.Compare(attribute.Value, targetValue, operatorType);

            VDebug.Log($"条件 {id} 未通过：战斗中未找到属性 {attributeName}。");
            return false;
        }
    }
}