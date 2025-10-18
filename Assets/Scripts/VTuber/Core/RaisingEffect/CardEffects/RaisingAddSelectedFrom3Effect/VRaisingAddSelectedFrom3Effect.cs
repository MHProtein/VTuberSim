using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;
using Random = UnityEngine.Random;

namespace VTuber.Core.RaisingEffect
{

    public class VRaisingAddSelectedFrom3Effect : VRaisingCardEffect
    {
        private VCardCondition _condition;
        private VCharacter _character;
        public VRaisingAddSelectedFrom3Effect(VRaisingAddSelectedFrom3EffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }
        
        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            _character = character;
            List<VCard> cardsToAdd = GetRandomCards(3, _condition, character.LiveType, character);
            
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginSelectCardFrom3, new Dictionary<string, object>()
            {
                {"Cards", cardsToAdd },
                {"ActionType", VCardActionType.Add}
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