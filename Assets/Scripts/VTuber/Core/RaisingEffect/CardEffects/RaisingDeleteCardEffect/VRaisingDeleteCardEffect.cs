using System.Collections.Generic;
using System.Linq;
using VTuber.Character;
using VTuber.Core.Foundation;
using Random = UnityEngine.Random;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingDeleteCardEffect : VRaisingEffect
    {
        private readonly VCardCondition _condition;

        public VRaisingDeleteCardEffect(VRaisingDeleteCardEffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            var cards = character.CardLibrary.GetCards();
            if (_condition is not null)
                cards = cards.Where(card => _condition.IsTrue(card)).ToList();
            var card = cards[Random.Range(0, cards.Count)];
            character.CardLibrary.RemoveCard(card);
            VDebug.Log("Deleted card: " + card.CardName);
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