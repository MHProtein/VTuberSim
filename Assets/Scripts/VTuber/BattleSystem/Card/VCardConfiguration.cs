using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine;
using VTuber.BattleSystem.Effect;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;

namespace VTuber.BattleSystem.Card
{
    public enum CostType
    {
        Stamina,
        TrueStamina,
        Buff
    }

    public enum VCardRarity
    {
        Basic, //white
        Common, //blue
        Rare, //purple
        Epic, //gold
        Special //black
    }

    public class VCardHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int DescriptionInGame = 3;
        public const int Rarity = 4;
        public const int Type = 5;
        public const int LiveType = 6;
        public const int Tag1 = 7;
        public const int Tag2 = 8;
        public const int Icon = 9;
        public const int CostType = 10;
        public const int CostBuffID = 11;
        public const int Cost = 12;
        public const int UpgradedCost = 13;
        public const int IsExhaust = 14;
        public const int NotRepeatable = 15;
        public const int Priority = 16;
        public const int Condition = 17;
        public const int Effect1 = 18;
        public const int E1Param = 19;
        public const int E1UpgradedParam = 20;
        public const int Effect2 = 21;
        public const int E2Param = 22;
        public const int E2UpgradedParam = 23;
        public const int Effect3 = 24;
        public const int E3Param = 25;
        public const int E3UpgradedParam = 26;
        public const int Effect4 = 27;
        public const int E4Param = 28;
        public const int E4UpgradedParam = 29;
        public const int NewEffect1 = 30;
        public const int NE1Param = 31;
        public const int NewEffect2 = 32;
        public const int NE2Param = 33;
    }

    [Serializable]
    public struct VEffectItem
    {
        public uint id;
        public string parameter;
        public string upgradedParameter;
        [HideInInspector] public int level;

        public VEffect CreateEffect()
        {
            if (VDataManager.Instance.EffectConfigurations.TryGetValue(id, out var config))
                return config.CreateEffect(parameter, upgradedParameter);

            VDebug.LogError($"Effect with ID {id} not found in configurations.");
            return null;
        }

        public VRaisingEffect CreateRaisingEffect()
        {
            if (VDataManager.Instance.RaisingEffects.TryGetValue(id, out var config))
                return config.CreateEffect(parameter, upgradedParameter);

            VDebug.LogError($"Effect with ID {id} not found in configurations.");
            return null;
        }
    }

    public class VCardConfiguration
    {
        public Sprite background;
        public string cardName;

        public string cardType;

        public int conditionId;
        public int cost;
        public uint costBuffId;

        public CostType costType = CostType.Stamina;

        public string description;

        public List<VEffectItem> effects;
        public Sprite facade;
        public Sprite icon;
        public uint id;
        public bool isExhaust;
        public string liveType;
        public List<VEffectItem> newEffects;
        public bool notRepeatable;
        public bool prioritized;

        public VCardRarity rarity;
        public List<string> tags;
        public int upgradedCost;
        public string upgradeDescription;

        public VCardConfiguration(CellRange row)
        {
            effects = new List<VEffectItem>();
            effects = new List<VEffectItem>();
            newEffects = new List<VEffectItem>();

            id = Convert.ToUInt32(row.Columns[VCardHeaderIndex.Id].Value.Trim());
            cardName = row.Columns[VCardHeaderIndex.Name].Value.Trim();
            var descriptions = row.Columns[VCardHeaderIndex.DescriptionInGame].Value.Trim().Split('$');
            description = descriptions[0];
            if (descriptions.Length > 1)
                upgradeDescription = descriptions[1];
            rarity = Enum.Parse<VCardRarity>(row.Columns[VCardHeaderIndex.Rarity].Value.Trim());
            cardType = row.Columns[VCardHeaderIndex.Type].Value.Trim();
            icon = VResourcesManager.Instance.TryGetSprite(row.Columns[VCardHeaderIndex.Icon].Value.Trim());
            liveType = row.Columns[VCardHeaderIndex.LiveType].Value.Trim();
            tags = new List<string>
            {
                row.Columns[VCardHeaderIndex.Tag1].Value.Trim(),
                row.Columns[VCardHeaderIndex.Tag2].Value.Trim()
            };

            costType = Enum.Parse<CostType>(row.Columns[VCardHeaderIndex.CostType].Value.Trim());

            if (costType == CostType.Buff)
                costBuffId = Convert.ToUInt32(row.Columns[VCardHeaderIndex.CostBuffID].Value.Trim());

            cost = Convert.ToInt32(row.Columns[VCardHeaderIndex.Cost].Value.Trim());
            upgradedCost = Convert.ToInt32(row.Columns[VCardHeaderIndex.UpgradedCost].Value.Trim());
            notRepeatable = Convert.ToInt32(row.Columns[VCardHeaderIndex.NotRepeatable].Value.Trim()) == 1;
            isExhaust = Convert.ToInt32(row.Columns[VCardHeaderIndex.IsExhaust].Value.Trim()) == 1;
            prioritized = !row.Columns[VCardHeaderIndex.Priority].Value.Trim().IsNullOrWhitespace();

            var conditionStr = row.Columns[VCardHeaderIndex.Condition].Value.Trim();
            if (!conditionStr.IsNullOrWhitespace())
                conditionId = Convert.ToInt32(conditionStr);
            else
                conditionId = -1;

            //background = VBattleDataManager.Instance.LoadSprite(csv.GetField<string>("Background"));
            //facade = VBattleDataManager.Instance.LoadSprite(csv.GetField<string>("Facade"));

            for (var i = VCardHeaderIndex.Effect1; i < VCardHeaderIndex.E4UpgradedParam; i += 3)
            {
                var effectIDStr = row.Columns[i].Value.Trim();
                if (effectIDStr.IsNullOrWhitespace())
                    continue;
                var effectID = Convert.ToUInt32(effectIDStr);

                if (VDataManager.Instance.EffectConfigurations.TryGetValue(effectID, out var config))
                {
                    var parameter = row.Columns[i + 1].Value;
                    var upgradedParameter = row.Columns[i + 2].Value;
                    effects.Add(new VEffectItem
                    {
                        id = effectID,
                        parameter = parameter,
                        upgradedParameter = upgradedParameter
                    });
                }
            }

            for (var i = VCardHeaderIndex.NewEffect1; i < VCardHeaderIndex.NE2Param; i += 2)
            {
                var effectIDStr = row.Columns[i].Value.Trim();
                if (effectIDStr.IsNullOrWhitespace())
                    continue;
                var effectID = Convert.ToUInt32(effectIDStr);

                if (VDataManager.Instance.EffectConfigurations.TryGetValue(effectID, out var config))
                {
                    var parameter = row.Columns[i + 1].Value;
                    newEffects.Add(new VEffectItem
                    {
                        id = effectID,
                        parameter = parameter,
                        upgradedParameter = parameter
                    });
                }
            }
        }


        public static uint IDDistributor { get; private set; }

        public static void LoadIDDistributor(uint id)
        {
            IDDistributor = id;
        }

        public VCard CreateCard()
        {
            //     if (_spawned)
            //         return null;
            //     _spawned = true;
            return new VCard(this, IDDistributor++, effects, newEffects, conditionId);
        }
    }
}