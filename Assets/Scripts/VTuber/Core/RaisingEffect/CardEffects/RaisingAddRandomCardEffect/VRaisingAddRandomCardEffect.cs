using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.UI;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddRandomCardEffect : VRaisingCardEffect
    {
        private VCardCondition _condition;
        public VRaisingAddRandomCardEffect(VRaisingAddRandomCardEffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            var card = GetRandomCards(1, _condition).FirstOrDefault();
            
            character.CardLibrary.AddCard(card);
            VDebug.Log("Added random card: " + card.CardName);
        }

        public override void Upgrade()
        {
            
        }

        public override void DownGrade()
        {
        }
    }
}