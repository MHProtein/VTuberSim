using System.Collections.Generic;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.Managers;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VAddSpecifiedCardEffect : VRaisingEffect
    {
        public VAddSpecifiedCardEffect(VRaisingEffectConfiguration configuration, string parameter) : base(
            configuration)
        {
            shouldPlayAnimation = false;
            CardId = uint.Parse(parameter);
        }

        public uint CardId { get; }
        private VCard _card;

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            if (animationRequest is not null)
            {
                _card = VDataManager.Instance.CreateCardByID(CardId);
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
            return VDataManager.Instance.GetCardConfigurationByID(CardId).cardName;
        }
    }
}