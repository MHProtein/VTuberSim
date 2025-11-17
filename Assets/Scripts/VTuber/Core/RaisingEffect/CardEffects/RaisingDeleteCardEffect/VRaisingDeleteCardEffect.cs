using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.RaisingAnimationSystem;
using Random = UnityEngine.Random;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingDeleteCardEffect : VRaisingEffect
    {
        private readonly VCardCondition _condition;
        private VCard _card;
        public VRaisingDeleteCardEffect(VRaisingDeleteCardEffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }
        
        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            var cards = character.CardLibrary.GetCards();
            if (_condition is not null)
                cards = cards.Where(card => _condition.IsTrue(card)).ToList();
            if (cards.Count == 0)
                return;
            _card = cards[Random.Range(0, cards.Count)];
            
            animationRequest.animationType = VAnimationType.RemoveCard;
            animationRequest.cards = new () { _card };
            animationRequest.instigatorType = VInstigatorType.Ignore;
            
            base.ApplyEffect(character, messagedict, animationRequest);
        }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.CardLibrary.RemoveCard(_card);
            VDebug.Log("Deleted card: " + _card.CardName);
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