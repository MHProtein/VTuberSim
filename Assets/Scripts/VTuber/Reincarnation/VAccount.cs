using System.Collections.Generic;
using VTuber.BattleSystem.Card;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;
using VTuber.Relic;

namespace VTuber.Reincarnation
{
    public class VAccount
    {
        private List<VCard> cards;
        private List<VRelic> relics;
        private List<VRaisingEffect> effects;
        
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