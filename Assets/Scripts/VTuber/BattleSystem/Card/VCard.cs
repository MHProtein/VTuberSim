using System.Collections.Generic;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Card
{
    public class VCard
    {
        public string CardName => _configuration.cardName;
        public bool IsExaust => _configuration.isExaust;
        
        private readonly VCardConfiguration _configuration;
        
        public VCard(VCardConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Play()
        {
            VDebug.Log("play card: " + CardName);
            VDebug.Log("effects: " + _configuration.effects.Count);
            Dictionary<string, object> message = new Dictionary<string, object>()
            {
                { "Card", this },
                { "Buffs", _configuration.buffs },
                { "Effects", _configuration.effects },
                { "Cost", _configuration.cost }
            };
            VRootEventCenter.Instance.Raise(VRootEventKey.OnCardPlayed, message);
        }
    }
}