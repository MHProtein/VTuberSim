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
using VTuber.Core.RaisingEffect;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Card
{   
    public enum CostType
    {
        Stamina,
        TrueStamina,
        Buff,
    }
    
    public enum VCardRarity
    {
        Basic, //white
        Common, //blue
        Rare,  //purple
        Epic,  //gold
        Special, //black
    }
    
    public class VCardHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int Rarity = 3;
        public const int Type = 4;
        public const int LiveType = 5;
        public const int Tag1 = 6;
        public const int Tag2 = 7;
        public const int CostType = 8;
        public const int CostBuffID = 9;
        public const int Cost = 10;
        public const int UpgradedCost = 11;
        public const int IsExhaust = 12;
        public const int NotRepeatable = 13;
        public const int Condition = 14;
        public const int Effect1 = 15;
        public const int E1Param = 16;
        public const int E1UpgradedParam = 17;
        public const int Effect2 = 18;
        public const int E2Param = 19;
        public const int E2UpgradedParam = 20;
        public const int Effect3 = 21;
        public const int E3Param = 22;
        public const int E3UpgradedParam = 23;
        public const int Effect4 = 24;
        public const int E4Param = 25;
        public const int E4UpgradedParam = 26;
        public const int NewEffect1 = 27;
        public const int NE1Param = 28;
        public const int NewEffect2 = 29;
        public const int NE2Param = 30;
    }

    public struct VEffectItem
    {
        public uint id;
        public string parameter;
        public string upgradedParameter;
        
        public VEffect CreateEffect()
        {
            if (VResourcesManager.Instance.EffectConfigurations.TryGetValue(id, out var config))
            {
                return config.CreateEffect(parameter, upgradedParameter);
            }
            else
            {
                VDebug.LogError($"Effect with ID {id} not found in configurations.");
                return null;
            }
        }

        public VRaisingEffect CreateRaisingEffect()
        {
            if (VResourcesManager.Instance.RaisingEffects.TryGetValue(id, out var config))
            {
                return config.CreateEffect(parameter, upgradedParameter);
            }
            else
            {
                VDebug.LogError($"Effect with ID {id} not found in configurations.");
                return null;
            }
        }
    }
    
    public class VCardConfiguration
    {
        public uint id;
        public string cardName;
        public string description;
        public string liveType;
        public List<string> tags;
        
        public string cardType;
        
        public VCardRarity rarity;
            
        public Sprite background;
        public Sprite facade;
        
        public CostType costType = CostType.Stamina;
        public uint costBuffId;
        public int cost;
        public int upgradedCost;
        public bool isExhaust = false;
        public bool notRepeatable = false;

        public List<VEffectItem> effects;
        public List<VEffectItem> newEffects;

        public int conditionId;
        

        private static uint idDistributor = 0;
        private bool spawned = false;
        public VCardConfiguration(CellRange row)
        {
            effects = new List<VEffectItem>();
            effects = new List<VEffectItem>();
            newEffects = new List<VEffectItem>();
            
            id = Convert.ToUInt32(row.Columns[VCardHeaderIndex.Id].Value.Trim());
            cardName = row.Columns[VCardHeaderIndex.Name].Value.Trim();
            description = row.Columns[VCardHeaderIndex.Description].Value.Trim();
            rarity = Enum.Parse<VCardRarity>(row.Columns[VCardHeaderIndex.Rarity].Value.Trim());
            cardType = row.Columns[VCardHeaderIndex.Type].Value.Trim();
            
            liveType = row.Columns[VCardHeaderIndex.LiveType].Value.Trim();
            tags = new List<string>
            {
                row.Columns[VCardHeaderIndex.Tag1].Value.Trim(),
                row.Columns[VCardHeaderIndex.Tag2].Value.Trim(),
            };
            
            costType = Enum.Parse<CostType>(row.Columns[VCardHeaderIndex.CostType].Value.Trim());
            
            if(costType == CostType.Buff)
                costBuffId = Convert.ToUInt32(row.Columns[VCardHeaderIndex.CostBuffID].Value.Trim());

            cost = Convert.ToInt32(row.Columns[VCardHeaderIndex.Cost].Value.Trim());
            upgradedCost = Convert.ToInt32(row.Columns[VCardHeaderIndex.UpgradedCost].Value.Trim());
            notRepeatable = Convert.ToInt32(row.Columns[VCardHeaderIndex.NotRepeatable].Value.Trim()) == 1;
            isExhaust = Convert.ToInt32(row.Columns[VCardHeaderIndex.IsExhaust].Value.Trim()) == 1;
            
            var conditionStr = row.Columns[VCardHeaderIndex.Condition].Value.Trim();
            if(!conditionStr.IsNullOrWhitespace())
            {
                conditionId = Convert.ToInt32(conditionStr);
            }
            else
            {
                conditionId = -1;
            }
            
            //background = VBattleDataManager.Instance.LoadSprite(csv.GetField<string>("Background"));
            //facade = VBattleDataManager.Instance.LoadSprite(csv.GetField<string>("Facade"));
            
            for (int i = VCardHeaderIndex.Effect1; i < VCardHeaderIndex.E4UpgradedParam; i += 3)
            {
                var effectIDStr = row.Columns[i].Value.Trim();
                if(effectIDStr.IsNullOrWhitespace())
                    continue;
                uint effectID = Convert.ToUInt32(effectIDStr);

                if (VResourcesManager.Instance.EffectConfigurations.TryGetValue(effectID, out var config))
                {
                    string parameter = row.Columns[i + 1].Value;
                    string upgradedParameter = row.Columns[i + 2].Value;
                    effects.Add(new VEffectItem(){
                        id = effectID,
                        parameter = parameter,
                        upgradedParameter = upgradedParameter
                    });
                }
            }
            
            for (int i = VCardHeaderIndex.NewEffect1; i < VCardHeaderIndex.NE2Param; i += 2)
            {
                var effectIDStr = row.Columns[i].Value.Trim();
                if(effectIDStr.IsNullOrWhitespace())
                    continue;
                uint effectID = Convert.ToUInt32(effectIDStr);

                if (VResourcesManager.Instance.EffectConfigurations.TryGetValue(effectID, out var config))
                {
                    string parameter = row.Columns[i + 1].Value;
                    newEffects.Add(new VEffectItem(){
                        id = effectID,
                        parameter = parameter,
                        upgradedParameter = parameter
                    });
                }
            }
        }
        
        public VCard CreateCard()
        {
        //     if (spawned)
        //         return null;
        //     spawned = true;
            return new VCard(this, idDistributor++, effects, newEffects, conditionId);
        }
    }
}