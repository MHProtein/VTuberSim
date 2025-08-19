using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.Foundation;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeRandomCardEffect : VRaisingCardEffect
    {
        public VRaisingUpgradeRandomCardEffect(VRaisingUpgradeRandomCardEffectConfiguration configuration) : base(configuration)
        {
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            base.ApplyEffect(character, messagedict);

            var cards = character.CardLibrary.GetCards().Where(vCard => !vCard.IsUpgraded).ToList();
            var card = cards[Random.Range(0, cards.Count)];
            card.Upgrade(false);
            VDebug.Log("Upgraded card: " + card.CardName);
        }
    }
}