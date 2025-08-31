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
        public List<VCard> Cards => cards;
        public List<VRelic> Relics => relics;
        public List<VRaisingEffect> Effects => effects;
        public List<int> EffectLevels => effectLevels;
        
        private List<VCard> cards;
        private List<VRelic> relics;
        private List<VRaisingEffect> effects;
        private List<int> effectLevels;
        
        public VAccount(List<VCard> cards, List<VRelic> relics, List<VEffectItem> effects)
        {
            this.cards = cards;
            this.relics = relics;
            
            this.effects = new List<VRaisingEffect>();
            foreach (var effect in effects)
            {
                this.effects.Add(effect.CreateRaisingEffect());
            }

            effectLevels = new List<int>();
            foreach (var effect in effects)
            {
                effectLevels.Add(effect.level);
            }
        }

        public VAccount(List<VCard> cards, List<VRelic> relics, List<VRaisingEffect> effects, List<int> effectLevels)
        {
            this.cards = cards;
            this.relics = relics;

            this.effects = effects;
            this.effectLevels = effectLevels;
        }
        
        public void Print()
        {
            string cardStr = "cards:";
            foreach (var card in cards)
            {
                cardStr += card.configID + ", ";
            }
            VDebug.Log(cardStr);
            
            string relicStr = "relics:";
            foreach (var relic in relics)
            {
                relicStr += relic.ConfigId + ", ";
            }
            VDebug.Log(relicStr);
            
            string effectStr = "effects:";
            foreach (var effect in effects)
            {
                effectStr += effect.Id + ", ";
            }
            VDebug.Log(effectStr);
        }
    }
}