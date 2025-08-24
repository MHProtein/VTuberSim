using System.Collections.Generic;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingSkipEventEffect : VRaisingEffect
    {
        private VUpgradableValue<int> _staminaRecovery;
        public VRaisingSkipEventEffect(VRaisingEffectConfiguration configuration, int staminaRecovery) : base(configuration)
        {
            _staminaRecovery = new VUpgradableValue<int>(staminaRecovery, staminaRecovery);
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSkipEvent, new Dictionary<string, object>()
            {
                {"StaminaRecovery", _staminaRecovery}
            });
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