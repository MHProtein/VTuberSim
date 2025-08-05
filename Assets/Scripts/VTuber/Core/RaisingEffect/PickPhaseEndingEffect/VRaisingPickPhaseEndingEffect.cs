using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect.PickPhaseEndingEffect
{
    public class VRaisingPickPhaseEndingEffect : VRaisingEffect
    {
        public VRaisingPickPhaseEndingEffect(VRaisingEffectConfiguration configuration) : base(configuration)
        {
            
        }

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSelectPhaseEndingBegin, new Dictionary<string, object>());
        }
    }
}