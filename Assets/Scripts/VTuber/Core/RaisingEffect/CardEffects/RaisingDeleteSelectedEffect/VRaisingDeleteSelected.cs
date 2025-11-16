using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingDeleteSelected : VRaisingEffect
    {
        private VCharacter _character;
        private VCardCondition _condition;

        public VRaisingDeleteSelected(VRaisingDeleteSelectedConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            var cards = character.CardLibrary.GetCards();
            if (cards.Count == 0)
                return;
            animationRequest.cards = cards;
            animationRequest.instigatorType = VInstigatorType.Ignore;
            animationRequest.animationType = VAnimationType.SelectCard;
            animationRequest.cardSelectable = true;
            animationRequest.returnable = false;
            animationRequest.cardSelectAnimationType = VAnimationType.RemoveCard;
            
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