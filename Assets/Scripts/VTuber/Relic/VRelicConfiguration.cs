using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.Managers;

namespace VTuber.Relic
{
    public static class VRelicHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int Type = 3;
        public const int Layer = 4;
        public const int WhenToApply = 5;
        public const int Condition = 6;
        public const int Effect1 = 7;
        public const int E1Param = 8;
        public const int E1UpgradedParam = 9;
        public const int Effect2 = 10;
        public const int E2Param = 11;
        public const int E2UpgradedParam = 12;
        public const int Effect3 = 13;
        public const int E3Param = 14;
        public const int E3UpgradedParam = 15;
        public const int Icon = 16;
    }
    
    public class VBattleRelicConfiguration : VRelicConfiguration
    {
        public List<VEffectItem> effectItems;
        public VEffectCondition condition;
        public VBattleEventKey whenToApply;
        public VBattleRelicConfiguration(CellRange row) : base(row)
        {
            string conditionStr = row.Columns[VCardHeaderIndex.Condition].Value;
            whenToApply = Enum.Parse<VBattleEventKey>(row.Columns[VRelicHeaderIndex.WhenToApply].Value.Trim());
            if (!string.IsNullOrEmpty(conditionStr))
            {
                condition = VResourcesManager.Instance.GetConditionByID(Convert.ToUInt32(conditionStr));
            }

            effectItems = new List<VEffectItem>();
            for (int i = VRelicHeaderIndex.Effect1; i <= VRelicHeaderIndex.E3Param; i += 2)
            {
                var effectIDStr = row.Columns[i].Value;
                if(effectIDStr.IsNullOrWhitespace())
                    continue;
                uint effect = Convert.ToUInt32(effectIDStr);
                
                effectItems.Add(new VEffectItem
                {
                    id = effect,
                    parameter = row.Columns[i + 1].Value,
                    upgradedParameter = row.Columns[i + 2].Value
                });
            }
        }
    }
    
    public class VRaisingRelicConfiguration : VRelicConfiguration
    {
        public List<VEffectItem> effectItems;
        public VRaisingRelicCondition condition;
        public VRaisingRelicConfiguration(CellRange row) : base(row)
        {
            effectItems = new List<VEffectItem>();
            
            string conditionStr = row.Columns[VCardHeaderIndex.Condition].Value;
            if (!string.IsNullOrEmpty(conditionStr))
            {
                condition = VResourcesManager.Instance.GetRaisingRelicCondition(Convert.ToUInt32(conditionStr));
            }
            
            for (int i = VRelicHeaderIndex.Effect1; i <= VRelicHeaderIndex.E3Param; i += 2)
            {               
                var effectIDStr = row.Columns[i].Value;
                if(effectIDStr.IsNullOrWhitespace())
                    continue;
                uint effect = Convert.ToUInt32(effectIDStr);
                
                effectItems.Add(new VEffectItem
                {
                    id = effect,
                    parameter = row.Columns[i + 1].Value,
                    upgradedParameter = row.Columns[i + 2].Value
                });
            }
        }

        public override VRelic CreateRelic()
        {
            return new VRelic(this);
        }
    }
    
    public class VRelicConfiguration
    {
        public uint id;
        public string relicName;
        public string description;
        public Sprite icon;
        public int layer;
        public bool isPermanent = false;

        public VRelicConfiguration(CellRange row)
        {
            id = Convert.ToUInt32(row.Columns[VRelicHeaderIndex.Id].Value);
            relicName = row.Columns[VRelicHeaderIndex.Name].Value;
            layer = Convert.ToInt32(row.Columns[VRelicHeaderIndex.Layer].Value);
            if (layer == -1)
                isPermanent = true;
        }

        public virtual VRelic CreateRelic()
        {
            return new VRelic(this);
        }
    }

}