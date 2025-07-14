using System;
using System.Collections.Generic;
using CsvHelper;
using Sirenix.OdinInspector;
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
    
    public class VCardConfiguration
    {
        public int id;
        public string cardName;
        public string description;
        
        public string cardType;
        
        public List<string> cardTags;
        public VCardRarity rarity;
            
        public Sprite background;
        public Sprite facade;
        
        public CostType costType = CostType.Stamina;
        public int costBuffId;
        public int cost;
        public bool isExaust = false;

        public List<int> effects;
        public Dictionary<int, int> upgradeEffects;

        private static int idDistributor = 0;

        public VCardConfiguration(CsvReader csv)
        {
            cardName = csv.GetField<string>("Name");
            description = csv.GetField<string>("Description");
            cardType = csv.GetField<string>("Type");
            cardTags = new List<string>();
            
            for (int i = 0; i < 5; i++)
            {
                cardTags.Add(csv.GetField<string>("Tag" + (i + 1)));
            }
            
            costType = Enum.Parse<CostType>(csv.GetField<string>("CostType"));
            if(costType == CostType.Buff)
                costBuffId = csv.GetField<int>("CostBuffID");
            cost = csv.GetField<int>("Cost");
            isExaust = csv.GetField<int>("IsExaust") == 1;
            rarity = Enum.Parse<VCardRarity>(csv.GetField<string>("Rarity"));
            
            //background = VBattleDataManager.Instance.LoadSprite(csv.GetField<string>("Background"));
            //facade = VBattleDataManager.Instance.LoadSprite(csv.GetField<string>("Facade"));
            
            effects = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                int? effect = csv.GetField<int?>("Effect" + (i + 1));
                if (effect.HasValue)
                {
                    effects.Add(effect.Value);
                }
            }

            upgradeEffects = new Dictionary<int, int>();
            for (int i = 4; i < 6; i++)
            {
                int? effect = csv.GetField<int?>($"Effect{i}_L1");
                if (effect.HasValue)
                {
                    upgradeEffects.Add(effect.Value, csv.GetField<int>($"Effect{i}_L2"));
                }
            }
        }
        
        public VCard CreateCard()
        {
            return new VCard(this, idDistributor++, CreateEffects());
        }
        
        protected List<VEffect> CreateEffects()
        {
            List<VEffect> effectList = new List<VEffect>();
            foreach (var effectId in effects)
            {

                if (VBattleDataManager.Instance.EffectConfigurations.TryGetValue(effectId, out var config))
                {
                    effectList.Add(config.CreateEffect());
                }
                else
                {
                    VDebug.LogError($"Effect with ID {effectId} not found for card {cardName}");
                }
            }
            return effectList;
        }
    }
}