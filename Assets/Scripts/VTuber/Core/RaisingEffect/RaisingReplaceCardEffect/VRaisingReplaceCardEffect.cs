using System.Linq;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.BattleSystem.Core.RaisingEffect
{
    public class VRaisingReplaceCardEffect : VRaisingDeleteCardEffect
    {
        public VRaisingReplaceCardEffect(VRaisingEffectConfiguration configuration) : base(configuration)
        {
            
        }

        public override void ApplyEffect(VCharacter character)
        {
        }
    }
}