using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingEnterStoreEffect : VRaisingEffect
    {
        public VRaisingEnterStoreEffect(VRaisingEnterStoreEffectConfiguration configuration) : base(configuration)
        {
            shouldPlayAnimation = false;
        }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRequestEnterStore,
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