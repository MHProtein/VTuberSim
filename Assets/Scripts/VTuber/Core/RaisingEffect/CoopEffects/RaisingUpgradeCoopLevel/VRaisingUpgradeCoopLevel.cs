using System.Collections.Generic;
using VTuber.Character;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeCoopLevel : VRaisingEffect
    {
        public uint cooperatorID;

        public VRaisingUpgradeCoopLevel(VRaisingUpgradeCoopLevelConfiguration configuration, string parameter) : base(
            configuration)
        {
            cooperatorID = uint.Parse(parameter);
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            animationRequest.coop = character.CooperatorManager.GetCooperator(cooperatorID);
            animationRequest.instigatorType = VInstigatorType.Ignore;
            animationRequest.animationType = VAnimationType.CoopUpgrade;
            base.ApplyEffect(character, messagedict, animationRequest);
        }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.CooperatorManager.GetCooperator(cooperatorID).UpgradeLevel();
        }

        public override void Upgrade()
        {
        }

        public override void DownGrade()
        {
        }

        public override string GetParameter()
        {
            return cooperatorID.ToString();
        }
    }
}