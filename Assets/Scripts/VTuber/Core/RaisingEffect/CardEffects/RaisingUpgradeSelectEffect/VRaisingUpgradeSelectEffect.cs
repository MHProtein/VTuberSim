using System.Collections.Generic;
using System.Linq;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeSelectEffect : VRaisingEffect
    {
        public VRaisingUpgradeSelectEffect(VRaisingEffectConfiguration configuration) : base(configuration)
        {
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            var cards = character.CardLibrary.GetCards().Where(vCard => !vCard.IsUpgraded).ToList();
            if (cards.Count == 0)
                return;
            animationRequest.cards = cards;
            animationRequest.instigatorType = VInstigatorType.Ignore;
            animationRequest.animationType = VAnimationType.SelectCardPreview;
            animationRequest.cardSelectable = true;
            animationRequest.returnable = false;
            animationRequest.cardSelectAnimationType = VAnimationType.UpgradeCard;
            animationRequest.cardSelectPreviewAction = card =>
            {
                card.Upgrade(true, false);
            };
            base.ApplyEffect(character, messagedict, animationRequest);
        }
        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventSelectUpgradeCard,
                new Dictionary<string, object>());
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