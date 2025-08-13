using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using Random = UnityEngine.Random;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingDeleteCardEffect : VRaisingCardEffect
    {
        private VCardCondition _condition;
        public VRaisingDeleteCardEffect(VRaisingDeleteCardEffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }

        public override void ApplyEffect(VCharacter character)
        {
            var card = GetRandomCards(1, _condition).FirstOrDefault();
            character.CardLibrary.RemoveCard(card);
            VDebug.Log("Deleted card: " + card.CardName);
        }

        public override void Upgrade()
        {
            
        }

        public override void DownGrade()
        {
        }
    }
}