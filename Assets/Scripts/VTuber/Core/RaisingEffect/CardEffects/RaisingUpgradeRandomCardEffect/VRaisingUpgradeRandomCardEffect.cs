using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeRandomCardEffect : VRaisingEffect
    {
        private VCard _card;
        public VRaisingUpgradeRandomCardEffect(VRaisingUpgradeRandomCardEffectConfiguration configuration) : base(
            configuration)
        {
            shouldPlayAnimation = false;
        }
        
        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            
            var cards = character.CardLibrary.GetCards().Where(vCard => !vCard.IsUpgraded).ToList();
            if (cards.Count == 0)
                return;
            _card = cards[Random.Range(0, cards.Count)];
            
            animationRequest.animationType = VAnimationType.UpgradeCard;
            animationRequest.cards = new () { _card };
            animationRequest.instigatorType = VInstigatorType.Ignore;
            
            base.ApplyEffect(character, messagedict, animationRequest);
        }

        public override void Upgrade()
        {
        }

        public override void DownGrade()
        {
        }

        public override string GetParameter()
        {
            return "";
        }
    }
}