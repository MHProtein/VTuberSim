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
        public List<VEffectItem> EffectItems => effectItems;
        
        private List<VCard> cards;
        private List<VRelic> relics;
        private List<VRaisingEffect> effects;
        private List<VEffectItem> effectItems;
        
        public VAccount(List<VCard> cards, List<VRelic> relics, List<VEffectItem> effects)
        {
            this.cards = cards;
            this.relics = relics;
            
            this.effects = new List<VRaisingEffect>();
            foreach (var effect in effects)
            {
                this.effects.Add(effect.CreateRaisingEffect());
            }

            effectItems = effects;
        }

        public VAccount(List<VCard> cards, List<VRelic> relics, List<VRaisingEffect> effects)
        {
            this.cards = cards;
            this.relics = relics;

            this.effects = effects;
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