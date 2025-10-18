using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.EventCenter;
using Random = UnityEngine.Random;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingReplaceCardEffect : VRaisingCardEffect
    {
        private VCardCondition _condition;
        private VCharacter _character;
        private VCard cardToReplace;
        public VRaisingReplaceCardEffect(VRaisingReplaceCardEffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            _character = character;
            cardToReplace = GetRandomCards(1, _condition, character.LiveType, character).FirstOrDefault();

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginSelectCard, new Dictionary<string, object>()
            {
                {"ActionType", VCardActionType.Replace}
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