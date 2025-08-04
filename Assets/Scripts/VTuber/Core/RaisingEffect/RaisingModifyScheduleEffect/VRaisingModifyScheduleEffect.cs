using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingModifyScheduleEffect : VRaisingEffect
    {
        public VRaisingModifyScheduleEffect(VRaisingEffectConfiguration configuration) : base(configuration)
        {
            
        }

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);
            
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSwitchToModifySchedule, new Dictionary<string, object>()
            {
            });
        }
    }
}