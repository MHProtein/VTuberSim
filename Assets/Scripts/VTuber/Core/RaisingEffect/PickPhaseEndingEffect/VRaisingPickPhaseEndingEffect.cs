using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingPickPhaseEndingEffect : VRaisingEffect
    {
        public VRaisingPickPhaseEndingEffect(VRaisingEffectConfiguration configuration) : base(configuration)
        {
            
        }

        public override void ApplyEffect(VCharacter character)
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSelectPhaseEndingBegin, new Dictionary<string, object>());
        }

        public override void Upgrade()
        {
            
        }

        public override void DownGrade()
        {
        }
    }
}