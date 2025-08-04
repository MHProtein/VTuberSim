using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingSkipEventEffect : VRaisingEffect
    {
        private int _staminaRecovery = 0;
        public VRaisingSkipEventEffect(VRaisingEffectConfiguration configuration, int staminaRecovery) : base(configuration)
        {
            _staminaRecovery = staminaRecovery;
        }

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSkipEvent, new Dictionary<string, object>()
            {
                {"StaminaRecovery", _staminaRecovery}
            });
        }
    }
}