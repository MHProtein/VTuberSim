using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.Managers;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddRelicEffect : VRaisingEffect
    {
        public VRaisingAddRelicEffect(VRaisingAddRelicEffectConfiguration configuration, string parameter,
            string upgradedParameter) : base(configuration)
        {
            RelicId = uint.Parse(parameter);
        }

        public uint RelicId { get; }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            animationRequest.relicId = RelicId;
            animationRequest.instigatorType = VInstigatorType.Ignore;
            animationRequest.animationType = VAnimationType.AddRelic;
            base.ApplyEffect(character, messagedict, animationRequest);
        }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.CharacterRelicManager.AddRelic
                (VDataManager.Instance.CreateRelicByID(RelicId));
        }

        public override void Upgrade()
        {
        }

        public override void DownGrade()
        {
        }

        public override string GetParameter()
        {
            return VDataManager.Instance.Relics[RelicId].relicName;
        }
    }
}