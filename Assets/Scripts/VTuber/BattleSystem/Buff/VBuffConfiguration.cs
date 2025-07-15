using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.StringToEnum;
using VTuber.Core.TypeSerialization;

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
        public const int Effect1 = 4;
        public const int E1Param = 5;
        public const int Effect2 = 6;
        public const int E2Param = 7;
        public const int Effect3 = 8;
        public const int E3Param = 9;
    }
    
    public class VBuffConfiguration
    {
        public int id;
        public string buffName;
        public Sprite icon;
        public BuffType buffType;
        public bool stackable = true;
        
        public List<VEffect> effects;

        public VBuffConfiguration(CellRange row)
        {
            id = Convert.ToInt32(row.Columns[VBuffHeaderIndex.Id].Value);
            buffName = row.Columns[VBuffHeaderIndex.Name].Value;
            //icon = csv.GetField<string>("Icon");
            buffType = Enum.Parse<BuffType>(row.Columns[VBuffHeaderIndex.BuffType].Value);
            stackable =  Convert.ToInt32(row.Columns[VBuffHeaderIndex.Stackable].Value) == 1;
            effects = new List<VEffect>();
            for (int i = VBuffHeaderIndex.Effect1; i <= VBuffHeaderIndex.E3Param; i += 2)
            {               
                var effectIDStr = row.Columns[i].Value;
                if(effectIDStr.IsNullOrWhitespace())
                    continue;
                int effect = Convert.ToInt32(effectIDStr);
                
                if (VBattleDataManager.Instance.EffectConfigurations.TryGetValue(effect, out var config))
                {
                    string parameter = row.Columns[i + 1].Value;
                    effects.Add(config.CreateEffect(parameter, parameter));
                }
            }
        }

        public VBuff CreateBuff()
        {
            return new VBuff(this, effects);
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