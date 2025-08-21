using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeSelectEffect : VRaisingEffect
    {
        public VRaisingUpgradeSelectEffect(VRaisingEffectConfiguration configuration) : base(configuration)
        {
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventSelectUpgradeCard, new Dictionary<string, object>());
        }

        public override void Upgrade()
        {
        }

        public override void DownGrade()
        {
        }
    }
}