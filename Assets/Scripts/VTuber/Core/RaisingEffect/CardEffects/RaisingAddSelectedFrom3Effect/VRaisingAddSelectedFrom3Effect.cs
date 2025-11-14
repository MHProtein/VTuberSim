using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

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

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            _character = character;
            var cardsToAdd = GetRandomCards(3, _condition, character.LiveType, character);

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginSelectCardFrom3,
                new Dictionary<string, object>
                {
                    { "Cards", cardsToAdd },
                    { "ActionType", VCardActionType.Add }
                });
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