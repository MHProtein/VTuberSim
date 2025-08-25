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
            cardToReplace = GetRandomCards(1, _condition).FirstOrDefault();

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginSelectCard, new Dictionary<string, object>()
            {
                {"Action", new Action<VCard>(ReplaceCard)}
            });
        }

        public override void Upgrade()
        {
            
        }

        public override void DownGrade()
        {
        }

        public void ReplaceCard(VCard selectedCard)
        {
            _character.CardLibrary.ReplaceCard(cardToReplace, selectedCard);
            
        }
        public override string GetParameter()
        {
            return "";
        }
        
    }
}