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
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
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
        public const int Icon = 6;
        public const int Condition = 7;
        public const int Effect1 = 8;
        public const int E1Param = 9;
        public const int E1UpgradedParam = 10;
        public const int Effect2 = 11;
        public const int E2Param = 12;
        public const int E2UpgradedParam = 13;
        public const int Effect3 = 14;
        public const int E3Param = 15;
        public const int E3UpgradedParam = 16;
    }
    
    public class VBattleRelicConfiguration : VRelicConfiguration
    {
        public readonly List<VEffectItem> effectItems;
        public readonly VEffectCondition condition;
        public readonly VBattleEventKey whenToApply;
        public VBattleRelicConfiguration(CellRange row) : base(row)
        {
            whenToApply = Enum.Parse<VBattleEventKey>(row.Columns[VRelicHeaderIndex.WhenToApply].Value.Trim());
            string conditionStr = row.Columns[VRelicHeaderIndex.Condition].Value;
            if (!string.IsNullOrEmpty(conditionStr))
            {
                condition = VDataManager.Instance.GetConditionByID(Convert.ToUInt32(conditionStr));
            }
            else
            {
                condition = null;
            }

            effectItems = new List<VEffectItem>();
            for (int i = VRelicHeaderIndex.Effect1; i <= VRelicHeaderIndex.E3UpgradedParam; i += 3)
            {
                var effectIDStr = row.Columns[i].Value;
                if(effectIDStr.IsNullOrWhitespace())
                    continue;
                uint effect = Convert.ToUInt32(effectIDStr);
                
                VDebug.Log(row.Columns[i + 1].Value + " "+ row.Columns[i + 2].Value);
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
            return new VBattleRelic(this);
        }
    }
    
    public class VRaisingRelicConfiguration : VRelicConfiguration
    {
        public readonly List<VEffectItem> effectItems;
        public readonly VRaisingRelicCondition condition;
        public readonly VRaisingEventKey whenToApply;
        public VRaisingRelicConfiguration(CellRange row) : base(row)
        {
            effectItems = new List<VEffectItem>();
            
            string conditionStr = row.Columns[VRelicHeaderIndex.Condition].Value;
            whenToApply = Enum.Parse<VRaisingEventKey>(row.Columns[VRelicHeaderIndex.WhenToApply].Value.Trim());
            if (!string.IsNullOrEmpty(conditionStr))
            {
                condition = VDataManager.Instance.GetRaisingRelicCondition(Convert.ToUInt32(conditionStr));
            }
            
            for (int i = VRelicHeaderIndex.Effect1; i <= VRelicHeaderIndex.E3UpgradedParam; i += 3)
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
            return new VRaisingRelic(this);
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
            description = row.Columns[VRelicHeaderIndex.Description].Value;
            icon = VResourcesManager.Instance.TryGetSprite(row.Columns[VRelicHeaderIndex.Icon].Value.Trim());
        }

        public virtual VRelic CreateRelic()
        {
            return new VRelic(this);
        }
    }

}