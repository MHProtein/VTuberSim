using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.EventCenter;
using Random = UnityEngine.Random;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingReplaceCardEffect : VRaisingEffect
    {
        private VCardCondition _condition;
        private VCharacter _character;
        private VCard cardToReplace;
        public VRaisingReplaceCardEffect(VRaisingReplaceCardEffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }

        public override void ApplyEffect(VCharacter character)
        {
            _character = character;
            List<VCard> cards = character.CardLibrary.GetCards().
                Where(card => _condition.IsTrue(card)).ToList();
            
            if (cards.Count == 0)
                return;
            int index = Random.Range(0, cards.Count);
            cardToReplace = cards[index];

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginSelectCard, new Dictionary<string, object>()
            {
                {"Action", new Action<VCard>(ReplaceCard)}
            });
        }
        
        public void ReplaceCard(VCard selectedCard)
        {
            _character.CardLibrary.ReplaceCard(cardToReplace, selectedCard);
            
        }
        
    }
}