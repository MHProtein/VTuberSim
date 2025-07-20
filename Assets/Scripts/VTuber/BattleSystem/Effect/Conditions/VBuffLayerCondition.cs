using System;
using System.Collections.Generic;
using Spire.Xls;
using UnityEditor.Sprites;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VBuffLayerCondition : VEffectCondition
    {
        public uint buffId;
        public int targetValue;

        public VBuffLayerCondition(CellRange row) : base(row)
        {
            buffId =  Convert.ToUInt32(row.Columns[VConditionHeaderIndex.NameOrID].Value);
            targetValue = ToInt( row.Columns[VConditionHeaderIndex.TargetValue].Value);
        }

        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            if (battle.BuffManager.TryGetBuff(buffId, out var buff))
            {
                bool result = Compare(buff.Value, targetValue);
                if (result)
                {
                    VDebug.Log($"条件 {id} 通过：ID为 {buffId} 的Buff层数为 {buff.Value}");
                }
                else
                {
                    VDebug.Log($"条件 {id} 未通过：ID为 {buffId} 的Buff层数为 {buff.Value}");
                }
                return result;
            }
            VDebug.Log($"条件 {id} 未通过：战斗中未找到ID为 {buffId} 的Buff。");
            return false;
        }
    }
}