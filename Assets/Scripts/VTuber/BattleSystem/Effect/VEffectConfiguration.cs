using System;
using System.Collections.Generic;
using CsvHelper;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect
{
    public class VEffectConfiguration
    {
        public int id;
        public string effectName;
        [TextArea] public string description;
        public bool multiplyByLayer = false;
        
        public VRootEventKey whenToApply;
        
        public VEffectCondition condition;

        public VEffectConfiguration(CsvReader csv)
        {
            id = csv.GetField<int>("ID");
            effectName = csv.GetField<string>("Name");
            description = csv.GetField<string>("Description");
            multiplyByLayer = csv.GetField<int>("MultiplyByLayer") == 1;
            string whenToApplyStr = csv.GetField<string>("WhenToApply");
            
            if (string.IsNullOrEmpty(whenToApplyStr))
                whenToApply = VRootEventKey.Default;
            else
                whenToApply = Enum.Parse<VRootEventKey>(whenToApplyStr);
            
            int? conditionId = csv.GetField<int?>("Condition");
            if(conditionId.HasValue)
                condition = VBattleDataManager.Instance.GetConditionByID(conditionId.Value);
        }
        
        public virtual VEffect CreateEffect()
        {
            return new VEffect(this);
        }
    }
}