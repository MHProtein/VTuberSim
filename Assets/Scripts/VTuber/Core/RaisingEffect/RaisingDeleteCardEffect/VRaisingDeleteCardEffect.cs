using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Character;

namespace VTuber.BattleSystem.Core.RaisingEffect
{
    public class VRaisingDeleteCardEffect : VRaisingEffect
    {
        private VCardCondition _condition;
        public VRaisingDeleteCardEffect(VRaisingEffectConfiguration configuration) : base(configuration)
        {
            
        }

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);
            List<VCard> cards = character.CardLibrary.GetCards().
                Where(card => _condition.IsTrue(card)).ToList();

            if (cards.Count == 0)
                return;
            int index = Random.Range(0, cards.Count);
            character.CardLibrary.RemoveCard(cards[index]);
        }
    }
}