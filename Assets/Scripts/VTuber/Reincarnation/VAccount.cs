using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Card;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.Relic;

namespace VTuber.Reincarnation
{
    public class VAccountSaveData
    {
        public string accountName;
        public List<uint> cards;
        public List<uint> relics;
        public List<VEffectItem> effects;
        public List<int> effectLevels;
        public string scoreLevel;
    }
    
    public class VAccount
    {
        public string accountName;
        public List<VCard> Cards => _cards;
        public List<VRelic> Relics => _relics;
        public List<VRaisingEffect> Effects => _effects;
        public List<int> EffectLevels => _effectLevels;
        public string ScoreLevel => _scoreLevel;
        
        private List<VCard> _cards;
        private List<VRelic> _relics;
        private List<VRaisingEffect> _effects;
        private List<int> _effectLevels;
        private string _scoreLevel;
        private List<VEffectItem> _effectItems;
        
        public VAccount(string scoreLevel, List<VCard> cards, List<VRelic> relics, List<VEffectItem> effects)
        {
            this._cards = cards;
            this._relics = relics;
            
            this._scoreLevel = scoreLevel;
            this._effectItems = effects;
            
            this._effects = new List<VRaisingEffect>();
            foreach (var effect in effects)
            {
                this._effects.Add(effect.CreateRaisingEffect());
            }

            _effectLevels = new List<int>();
            foreach (var effect in effects)
            {
                _effectLevels.Add(effect.level);
            }
        }

        public VAccount(VAccountSaveData data)
        {
            this.accountName = data.accountName;
            this._scoreLevel = data.scoreLevel;
            this._cards = data.cards.Select(card => VDataManager.Instance.CreateCardByID(card)).ToList();
            this._relics = data.relics.Select(relic => VDataManager.Instance.CreateRelicByID(relic)).ToList();
            this._effectLevels = data.effectLevels;
            _effectItems = data.effects;
            this._effects = new List<VRaisingEffect>();
            foreach (var effect in this._effectItems)
            {
                this._effects.Add(effect.CreateRaisingEffect());
            }
        }

        public VAccount(string scoreLevel, List<VCard> cards, List<VRelic> relics, List<VRaisingEffect> effects, List<int> effectLevels)
        {
            this._scoreLevel = scoreLevel;
            this._cards = cards;
            this._relics = relics;

            this._effects = effects;
            this._effectLevels = effectLevels;
        }

        public VAccountSaveData Save()
        {
            return new VAccountSaveData
            {
                accountName = accountName,
                cards = _cards.Select(card => card.configID).ToList(),
                relics = _relics.Select(relic => relic.ConfigId).ToList(),
                effects = _effectItems,
                effectLevels = _effectLevels,
                scoreLevel = _scoreLevel
            };
        }
    }
}