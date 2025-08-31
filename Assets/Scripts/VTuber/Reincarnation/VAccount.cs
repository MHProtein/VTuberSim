using System.Collections.Generic;
using VTuber.BattleSystem.Card;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;
using VTuber.Relic;

namespace VTuber.Reincarnation
{
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
        
        public VAccount(string scoreLevel, List<VCard> cards, List<VRelic> relics, List<VEffectItem> effects)
        {
            this._cards = cards;
            this._relics = relics;
            
            this._scoreLevel = scoreLevel;
            
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

        public VAccount(string scoreLevel, List<VCard> cards, List<VRelic> relics, List<VRaisingEffect> effects, List<int> effectLevels)
        {
            this._scoreLevel = scoreLevel;
            this._cards = cards;
            this._relics = relics;

            this._effects = effects;
            this._effectLevels = effectLevels;
        }
        
        public void Print()
        {
            string cardStr = "cards:";
            foreach (var card in _cards)
            {
                cardStr += card.configID + ", ";
            }
            VDebug.Log(cardStr);
            
            string relicStr = "relics:";
            foreach (var relic in _relics)
            {
                relicStr += relic.ConfigId + ", ";
            }
            VDebug.Log(relicStr);
            
            string effectStr = "effects:";
            foreach (var effect in _effects)
            {
                effectStr += effect.Id + ", ";
            }
            VDebug.Log(effectStr);
        }
    }
}