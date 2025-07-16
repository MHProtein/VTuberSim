using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Effect;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Card
{   
    public enum CostType
    {
        Stamina,
        Buff,
    }
    
    public enum VCardRarity
    {
        Common, //blue
        Rare,  //purple
        Epic,  //gold
    }
    
    public class VCardHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int Rarity = 3;
        public const int Type = 4;
        public const int CostType = 5;
        public const int CostBuffID = 6;
        public const int Cost = 7;
        public const int UpgradedCost = 8;
        public const int IsExhaust = 9;
    
        public const int Effect1 = 10;
        public const int E1Param = 11;
        public const int E1UpgradedParam = 12;
    
        public const int Effect2 = 13;
        public const int E2Param = 14;
        public const int E2UpgradedParam = 15;
    
        public const int Effect3 = 16;
        public const int E3Param = 17;
        public const int E3UpgradedParam = 18;
    
        public const int Effect4 = 19;
        public const int E4Param = 20;
        public const int E4UpgradedParam = 21;

        public const int NewEffect1 = 22;
        public const int NE1Param = 23;
        public const int NewEffect2 = 24;
        public const int NE2Param = 25;
    }

    public struct VCardEffectItem
    {
        public uint id;
        public string parameter;
        public string upgradedParameter;
    }
    
    public class VCardConfiguration
    {
        public uint id;
        public string cardName;
        public string description;
        
        public string cardType;
        
        public VCardRarity rarity;
            
        public Sprite background;
        public Sprite facade;
        
        public CostType costType = CostType.Stamina;
        public uint costBuffId;
        public int cost;
        public int upgradedCost;
        public bool IsExhaust = false;

        public List<VCardEffectItem> effects;
        public List<VCardEffectItem> newEffects;
        

        private static uint idDistributor = 0;

        public VCardConfiguration(CellRange row)
        {
            effects = new List<VCardEffectItem>();
            effects = new List<VCardEffectItem>();
            newEffects = new List<VCardEffectItem>();
            
            
            id = Convert.ToUInt32(row.Columns[VCardHeaderIndex.Id].Value);
            cardName = row.Columns[VCardHeaderIndex.Name].Value;
            description = row.Columns[VCardHeaderIndex.Description].Value;
            rarity = Enum.Parse<VCardRarity>(row.Columns[VCardHeaderIndex.Rarity].Value);
            cardType = row.Columns[VCardHeaderIndex.Type].Value;
            
            costType = Enum.Parse<CostType>(row.Columns[VCardHeaderIndex.CostType].Value);
            
            if(costType == CostType.Buff)
                costBuffId = Convert.ToUInt32(row.Columns[VCardHeaderIndex.Id].Value);

            cost = Convert.ToInt32(row.Columns[VCardHeaderIndex.Cost].Value);
            upgradedCost = Convert.ToInt32(row.Columns[VCardHeaderIndex.UpgradedCost].Value);
            
            IsExhaust = Convert.ToInt32(row.Columns[VCardHeaderIndex.IsExhaust].Value) == 1;
            
            //background = VBattleDataManager.Instance.LoadSprite(csv.GetField<string>("Background"));
            //facade = VBattleDataManager.Instance.LoadSprite(csv.GetField<string>("Facade"));
            
            for (int i = VCardHeaderIndex.Effect1; i < VCardHeaderIndex.E4UpgradedParam; i += 3)
            {
                var effectIDStr = row.Columns[i].Value;
                if(effectIDStr.IsNullOrWhitespace())
                    continue;
                uint effectID = Convert.ToUInt32(effectIDStr);

                if (VBattleDataManager.Instance.EffectConfigurations.TryGetValue(effectID, out var config))
                {
                    string parameter = row.Columns[i + 1].Value;
                    string upgradedParameter = row.Columns[i + 2].Value;
                    effects.Add(new VCardEffectItem(){
                        id = effectID,
                        parameter = parameter,
                        upgradedParameter = upgradedParameter
                    });
                }
            }
            
            for (int i = VCardHeaderIndex.NewEffect1; i < VCardHeaderIndex.NE2Param; i += 2)
            {
                var effectIDStr = row.Columns[i].Value;
                if(effectIDStr.IsNullOrWhitespace())
                    continue;
                uint effectID = Convert.ToUInt32(effectIDStr);

                if (VBattleDataManager.Instance.EffectConfigurations.TryGetValue(effectID, out var config))
                {
                    string parameter = row.Columns[i + 1].Value;
                    newEffects.Add(new VCardEffectItem(){
                        id = effectID,
                        parameter = parameter,
                        upgradedParameter = string.Empty
                    });
                }
            }
        }
        
        public VCard CreateCard()
        {
            return new VCard(this, idDistributor++, effects, newEffects);
        }
    }
}