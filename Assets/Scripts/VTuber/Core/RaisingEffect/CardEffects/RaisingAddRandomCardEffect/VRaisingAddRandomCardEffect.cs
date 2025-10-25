using System.Collections.Generic;
using System.Linq;
using VTuber.Character;
using VTuber.Core.Foundation;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddRandomCardEffect : VRaisingCardEffect
    {
        private readonly VCardCondition _condition;

        public VRaisingAddRandomCardEffect(VRaisingAddRandomCardEffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            var card = GetRandomCards(1, _condition, character.LiveType, character).FirstOrDefault();

            character.CardLibrary.AddCard(card);
            VDebug.Log("Added random card: " + card.CardName);
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