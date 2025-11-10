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

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSelectPhaseEndingBegin,
                new Dictionary<string, object>());
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