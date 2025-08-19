using System.Collections.Generic;
using System.Linq;
using VTuber.Character;
using VTuber.Core.Foundation;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeRandomCardEffect : VRaisingCardEffect
    {
        private VCardCondition _condition;
        public VRaisingUpgradeRandomCardEffect(VRaisingUpgradeRandomCardEffectConfiguration configuration) : base(configuration)
        {
            _condition = configuration.Condition;
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            base.ApplyEffect(character, messagedict);
            
            var card = GetRandomCards(1, _condition).FirstOrDefault();
            
            card.Upgrade(false);
        }
    }
}