using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddRandomCardEffect : VRaisingCardEffect
    {
        private readonly VCardCondition _condition;
        private VCard _card;
        public VRaisingAddRandomCardEffect(VRaisingAddRandomCardEffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }
        
        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            _card = GetRandomCards(1, _condition, character.LiveType, character).FirstOrDefault();
            if (animationRequest is not null)
            {
                animationRequest.animationType = VAnimationType.AddCard;
                animationRequest.cards = new() { _card };
                animationRequest.instigatorType = VInstigatorType.Ignore;
            }

            base.ApplyEffect(character, messagedict, animationRequest);
        }
        
        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.CardLibrary.AddCard(_card);
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