using System;
using System.Collections.Generic;
using Spire.Xls;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect
{
    public class VEffectHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int Type = 3;
        public const int WhenToApply = 4;
        public const int Condition1 = 5;
        public const int Condition2 = 6;
        public const int MultiplyByLayer = 7;
        public const int Parameter = 8;
    }
    
    public class VEffectConfiguration
    { 
        public uint id;
        public string effectName;
        [TextArea] public string description;
        public float multiplyByLayer = 0.0f;
        
        public VRootEventKey whenToApply;
        
        public List<VEffectCondition> conditions;

        public VEffectConfiguration(CellRange row)
        {
            id = Convert.ToUInt32(row.Columns[VEffectHeaderIndex.Id].Value);
            effectName = row.Columns[VEffectHeaderIndex.Name].Value;
            description = row.Columns[VEffectHeaderIndex.Description].Value;
            multiplyByLayer = Convert.ToSingle(row.Columns[VEffectHeaderIndex.MultiplyByLayer].Value);
            
            string whenToApplyStr = row.Columns[VEffectHeaderIndex.WhenToApply].Value;
            if (string.IsNullOrEmpty(whenToApplyStr))
                whenToApply = VRootEventKey.Default;
            else
                whenToApply = Enum.Parse<VRootEventKey>(whenToApplyStr);
            
            conditions = new List<VEffectCondition>();
            for (int i = VEffectHeaderIndex.Condition1; i <= VEffectHeaderIndex.Condition2; i++)
            {
                string conditionStr = row.Columns[i].Value;
                if (string.IsNullOrEmpty(conditionStr))
                    continue;
                
                conditions.Add(VBattleDataManager.Instance.GetConditionByID(Convert.ToUInt32(conditionStr)));
            }
        }
        
        public virtual VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VEffect(this);
        }
    }
}