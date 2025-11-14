using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingReplaceCardEffect : VRaisingCardEffect
    {
        private readonly VCardCondition _condition;
        private VCharacter _character;
        private VCard cardToReplace;

        public VRaisingReplaceCardEffect(VRaisingReplaceCardEffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            _character = character;
            cardToReplace = GetRandomCards(1, _condition, character.LiveType, character).FirstOrDefault();

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginSelectCard, new Dictionary<string, object>
            {
                { "ActionType", VCardActionType.Replace }
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