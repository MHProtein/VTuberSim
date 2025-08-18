using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingEnterStoreEffect : VRaisingEffect
    {
        public VRaisingEnterStoreEffect(VRaisingEnterStoreEffectConfiguration configuration) : base(configuration)
        {
            
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRequestEnterStore, new Dictionary<string, object>());
        }

        public override void Upgrade()
        {
            throw new System.NotImplementedException();
        }

        public override void DownGrade()
        {
            throw new System.NotImplementedException();
        }
    }
}