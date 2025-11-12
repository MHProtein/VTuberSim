using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingEndingEffect : VRaisingEffect
    {
        public VRaisingEndingEffect(VRaisingEffectConfiguration configuration) : base(configuration)
        {
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginEnding, new Dictionary<string, object>());
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