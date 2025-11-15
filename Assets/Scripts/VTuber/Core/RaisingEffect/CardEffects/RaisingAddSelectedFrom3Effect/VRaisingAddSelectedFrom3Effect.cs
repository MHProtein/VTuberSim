using System.Collections.Generic;
using System.Linq;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddSelectedFrom3Effect : VRaisingCardEffect
    {
        private readonly VCardCondition _condition;
        private VCharacter _character;

        public VRaisingAddSelectedFrom3Effect(VRaisingAddSelectedFrom3EffectConfiguration configuration) : base(
            configuration)
        {
            _condition = configuration.Condition;
        }
        
        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            animationRequest.animationType = VAnimationType.SelectCardFrom3;
            animationRequest.cards = GetRandomCards(3, _condition, character.LiveType, character);
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