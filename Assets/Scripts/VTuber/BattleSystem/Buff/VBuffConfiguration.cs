using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine;
using VTuber.BattleSystem.Card;

namespace VTuber.BattleSystem.Buff
{
    public enum BuffType
    {
        Permanent,
        Persistent
    }

    public class VBuffHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int BuffType = 3;
        public const int Stackable = 4;
        public const int Latency = 5;
        public const int Icon = 6;
        public const int ReduceLayerInFirstTurn = 7; // 第一回合减少层数
        public const int Effect1 = 8;
        public const int E1Param = 9;
        public const int Effect2 = 10;
        public const int E2Param = 11;
        public const int Effect3 = 12;
        public const int E3Param = 13;
        public const int Effect4 = 14;
        public const int E4Param = 15;
        public const int Effect5 = 16;
        public const int E5Param = 17;
    }

    //Buff 的配置数据通过 VBuffConfiguration 存储，并延迟用于创建实例
    public class VBuffConfiguration
    {
        public string buffName;
        public BuffType buffType;
        public string description;
        public bool shouldFirstTurnDecrementLayer;

        public List<VEffectItem> effectItems;
        public Sprite icon;
        public uint id;
        public int latency;
        public bool stackable = true;

        // 解析表格数据构建配置
        public VBuffConfiguration(CellRange row)
        {
            id = Convert.ToUInt32(row.Columns[VBuffHeaderIndex.Id].Value);
            buffName = row.Columns[VBuffHeaderIndex.Name].Value;
            description = row.Columns[VBuffHeaderIndex.Description].Value;
            buffType = Enum.Parse<BuffType>(row.Columns[VBuffHeaderIndex.BuffType].Value);
            stackable = Convert.ToInt32(row.Columns[VBuffHeaderIndex.Stackable].Value) == 1;
            effectItems = new List<VEffectItem>();
            latency = Convert.ToInt32(row.Columns[VBuffHeaderIndex.Latency].Value);
            icon = VResourcesManager.Instance.TryGetSprite(row.Columns[VBuffHeaderIndex.Icon].Value.Trim());
            shouldFirstTurnDecrementLayer = Convert.ToInt32(row.Columns[VBuffHeaderIndex.ReduceLayerInFirstTurn].Value) == 1;
            
            // 每两个字段为一组：EffectID + 参数
            for (var i = VBuffHeaderIndex.Effect1; i <= VBuffHeaderIndex.E5Param; i += 2)
            {
                var effectIDStr = row.Columns[i].Value;
                if (effectIDStr.IsNullOrWhitespace())
                    continue;
                var effect = Convert.ToUInt32(effectIDStr);

                //每个 Buff 可绑定多个效果，通过 VEffectItem 创建出 VEffect
                effectItems.Add(new VEffectItem
                {
                    id = effect,
                    parameter = row.Columns[i + 1].Value,
                    upgradedParameter = row.Columns[i + 1].Value
                });
            }
        }

        // 创建Buff实例
        public VBuff CreateBuff()
        {
            return new VBuff(this, effectItems.Select(item => item.CreateEffect()).ToList());
        }

        public bool IsBuffPersistent()
        {
            return buffType == BuffType.Persistent;
        }

        public bool IsBuffPermanent()
        {
            return buffType == BuffType.Permanent;
        }
    }
}