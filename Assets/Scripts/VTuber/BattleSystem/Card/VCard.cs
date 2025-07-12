using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Card
{
    public enum VCardRarity
    {
        Common, //blue
        Rare,  //purple
        Epic,  //gold
    }
    public class VCard
    {
        public int Id { get; private set; }
        public string CardName => _configuration.cardName;
        public bool IsExaust => _configuration.isExaust;
        public string CardType => _configuration.cardType;
        public string Description => _configuration.description;
        public List<string> CardTags => _configuration.cardTags;
        public CostType CostType => _configuration.costType;
        public int CostBuffId => _configuration.costBuffId;
        public int Cost => _configuration.cost;
        public Sprite Background => _configuration.background;
        public Sprite Facade => _configuration.facade;
        public List<VEffectConfiguration> Effects => _configuration.effects;
        public VCardRarity Rarity => _configuration.rarity;
        
        public bool isPlayable;
        
        private readonly VCardConfiguration _configuration;
        
        public VCard(VCardConfiguration configuration, int id)
        {
            _configuration = configuration;
            Id = id;
        }

        public void Play()
        {
            VDebug.Log("play card: " + CardName);
            VDebug.Log("effects: " + _configuration.effects.Count);
            Dictionary<string, object> message = new Dictionary<string, object>()
            {
                { "Card", this },
                { "Effects", _configuration.effects },
                { "Cost", _configuration.cost },
                {"CostType", CostType},
                {"CostBuffId", CostBuffId}
            };
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnCardPlayed, message);
        }
    }
}