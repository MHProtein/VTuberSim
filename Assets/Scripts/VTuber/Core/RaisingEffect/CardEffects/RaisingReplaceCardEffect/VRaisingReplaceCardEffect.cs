using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingReplaceCardEffect : VRaisingCardEffect
    {
        private readonly VCardCondition _condition;

        public VRaisingReplaceCardEffect(VRaisingReplaceCardEffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }
        
        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            animationRequest.previewCard = GetRandomCards(1, _condition, character.LiveType, character).FirstOrDefault();
            if ( animationRequest.previewCard is null || character.CardLibrary.GetCards().Count == 0)
                return;

            animationRequest.cards = character.CardLibrary.GetCards();
            animationRequest.animationType = VAnimationType.SelectCardPreview;
            animationRequest.cardSelectable = true;
            animationRequest.returnable = false;
            animationRequest.cardSelectAnimationType = VAnimationType.ReplaceCard;
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