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
    public class VRaisingAddSelectedFrom3Effect : VRaisingEffect
    {
        private VCardCondition _condition;
        private VCharacter _character;
        public VRaisingAddSelectedFrom3Effect(VRaisingAddSelectedFrom3EffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }
        
        public override void ApplyEffect(VCharacter character)
        {
            _character = character;
            List<VCardConfiguration> cards = VResourcesManager.Instance.GetAllCardConfigurations().
                Where(card => _condition.IsTrue(card)).ToList();
            
            if (cards.Count == 0)
                return;
            List<VCard> cardsToAdd = new List<VCard>();
            for (int i = 0; i < 3; i++)
            {
                int index = Random.Range(0, cards.Count);
                cardsToAdd.Add(cards[index].CreateCard());
            }
            
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginSelectCardFrom3, new Dictionary<string, object>()
            {
                {"Cards", cardsToAdd },
                {"Action", new Action<VCard>(ReplaceCard)}
            });
        }
        
        public void ReplaceCard(VCard selectedCard)
        {
            _character.CardLibrary.AddCard(selectedCard);
        }
    }
}