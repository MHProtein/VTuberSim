using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.BattleSystem.Card
{

    public class VCard
    {
        public uint Id { get; private set; }
        public string CardName => _configuration.cardName;
        public bool IsExhaust => _configuration.IsExhaust;
        public string CardType => _configuration.cardType;
        public string Description => _configuration.description;
        public CostType CostType => _configuration.costType;
        public uint CostBuffId => _configuration.costBuffId;
        
        public int Cost => _cost.Value;
        private VUpgradableValue<int> _cost;
        
        public Sprite Background => _configuration.background;
        public Sprite Facade => _configuration.facade;
        
        private List<VEffect> _effects;
        private List<VEffect> _newEffects;

        public bool IsUpgraded => _isUpgraded;
        public bool IsTemporaryUpgraded => isTemporaryUpgraded;
        
        private bool _isUpgraded = false;
        private bool isTemporaryUpgraded = false;
        
        public List<VEffect> Effects
        {
            get
            {
                List<VEffect> effects = new List<VEffect>();
                effects.AddRange(_effects);
                if (_isUpgraded)
                {
                    effects.AddRange(_newEffects);
                }
                return effects;
            }
        }
        public VCardRarity Rarity => _configuration.rarity;
        
        public Action<bool> SetPlayable;
        
        private readonly VCardConfiguration _configuration;
        
        public VCard(VCardConfiguration configuration, uint id, List<VCardEffectItem> effects, List<VCardEffectItem> newEffects)
        {
            _configuration = configuration;
            Id = id;
            _effects = new List<VEffect>();
            _effects = new List<VEffect>();
            _newEffects = new List<VEffect>();
            _cost = new VUpgradableValue<int>(configuration.cost, configuration.upgradedCost);
            foreach (var effect in effects)
            {
                _effects.Add(VBattleDataManager.Instance.CreateEffectByID(effect.id, effect.parameter, effect.parameter));
            }
            
            foreach (var effect in newEffects)
            {
                _newEffects.Add(VBattleDataManager.Instance.CreateEffectByID(effect.id, effect.parameter, effect.upgradedParameter));
            }
        }

        public void Play()
        {
            VDebug.Log("play card: " + CardName);
            VDebug.Log("effects: " + _configuration.effects.Count);
            Dictionary<string, object> message = new Dictionary<string, object>()
            {
                { "Card", this },
                { "Effects", Effects },
                { "Cost", Cost },
                { "CostType", CostType },
                { "CostBuffId", CostBuffId }
            };
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardPlayed, message);
        }

        public void Upgrade(bool isTemporary)
        {
            if (_isUpgraded)
            {
                return;
            }
            
            _isUpgraded = true;
            isTemporaryUpgraded = isTemporary;
            _cost.Upgrade();
            
            foreach (var effect in _effects)
            {
                effect.Upgrade();
            }
        }

        public void Downgrade()
        {
            if (!_isUpgraded)
            {
                return;
            }
            
            _isUpgraded = false;
            isTemporaryUpgraded = false;
            _cost.Downgrade();
            
            foreach (var effect in _effects)
            {
                effect.Downgrade();
            }
        }
        
    }
}