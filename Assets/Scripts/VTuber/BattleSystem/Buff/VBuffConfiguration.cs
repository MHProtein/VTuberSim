using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine;
using VTuber.BattleSystem.Card;

namespace VTuber.BattleSystem.Buff
{
    public enum BuffType
    {
        Permanent,
        Persistent,
    }
    
    public class VBuffHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int BuffType = 2;
        public const int Stackable = 3;
        public const int Latency = 4;
        public const int Effect1 = 5;
        public const int E1Param = 6;
        public const int Effect2 = 7;
        public const int E2Param = 8;
        public const int Effect3 = 9;
        public const int E3Param = 10;
    }
    
    public class VBuffConfiguration
    {
        public uint id;
        public string buffName;
        public Sprite icon;
        public BuffType buffType;
        public bool stackable = true;
        public int latency = 0;
        
        public List<VEffectItem> effectItems;

        public VBuffConfiguration(CellRange row)
        {
            id = Convert.ToUInt32(row.Columns[VBuffHeaderIndex.Id].Value);
            buffName = row.Columns[VBuffHeaderIndex.Name].Value;
            //icon = csv.GetField<string>("Icon");
            buffType = Enum.Parse<BuffType>(row.Columns[VBuffHeaderIndex.BuffType].Value);
            stackable =  Convert.ToInt32(row.Columns[VBuffHeaderIndex.Stackable].Value) == 1;
            effectItems = new List<VEffectItem>();
            latency = Convert.ToInt32(row.Columns[VBuffHeaderIndex.Latency].Value);
            for (int i = VBuffHeaderIndex.Effect1; i <= VBuffHeaderIndex.E3Param; i += 2)
            {               
                var effectIDStr = row.Columns[i].Value;
                if(effectIDStr.IsNullOrWhitespace())
                    continue;
                uint effect = Convert.ToUInt32(effectIDStr);
                
                effectItems.Add(new VEffectItem
                {
                    id = effect,
                    parameter = row.Columns[i + 1].Value,
                    upgradedParameter = row.Columns[i + 1].Value
                });
            }
        }

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