using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Card
{

    public class VCard
    {
        public int Id { get; private set; }
        public string CardName => _configuration.cardName;
        public bool IsExhaust => _configuration.IsExhaust;
        public string CardType => _configuration.cardType;
        public string Description => _configuration.description;
        public CostType CostType => _configuration.costType;
        public int CostBuffId => _configuration.costBuffId;
        public int Cost => _configuration.cost.Value;
        public Sprite Background => _configuration.background;
        public Sprite Facade => _configuration.facade;
        
        public List<VEffect> Effects;
        public VCardRarity Rarity => _configuration.rarity;
        
        public Action<bool> SetPlayable;
        
        private readonly VCardConfiguration _configuration;
        
        public VCard(VCardConfiguration configuration, int id, List<VEffect> effects)
        {
            _configuration = configuration;
            Id = id;
            Effects = effects;
        }

        public void Play()
        {
            VDebug.Log("play card: " + CardName);
            VDebug.Log("effects: " + _configuration.effects.Count);
            Dictionary<string, object> message = new Dictionary<string, object>()
            {
                { "Card", this },
                { "Effects", _configuration.effects },
                { "Cost", _configuration.cost.Value },
                { "CostType", CostType },
                { "CostBuffId", CostBuffId }
            };
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnCardPlayed, message);
        }
    }
}