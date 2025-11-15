using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddSelectFrom3ConsumableEffect : VRaisingConsumableEffect
    {
        private VCharacter _character;

        public VRaisingAddSelectFrom3ConsumableEffect(VRaisingConsumableEffectConfiguration configuration) : base(
            configuration)
        {
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            animationRequest.consumableIDs = GetRandomConsumables(3, character.LiveType).Select(c => c.ConfigId).ToList();
            animationRequest.animationType = VAnimationType.SelectConsumableFrom3;
            animationRequest.instigatorType = VInstigatorType.Ignore;
            base.ApplyEffect(character, messagedict, animationRequest);
        }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
        }

        public override void Upgrade()
        {
        }

        public override void DownGrade()
        {
        }

        public void AddConsumable(VConsumable consumable)
        {
            _character.ConsumableManager.AddConsumable(consumable);
        }

        public override string GetParameter()
        {
            return "";
        }
    }
}