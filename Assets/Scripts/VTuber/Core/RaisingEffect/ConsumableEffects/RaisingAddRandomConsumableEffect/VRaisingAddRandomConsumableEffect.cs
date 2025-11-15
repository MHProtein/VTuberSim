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
    public class VRaisingAddRandomConsumableEffect : VRaisingConsumableEffect
    {
        private VCharacter _character;
        private VConsumable _consumable;

        public VRaisingAddRandomConsumableEffect(VRaisingConsumableEffectConfiguration configuration) : base(
            configuration)
        {
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            _consumable = GetRandomConsumables(1, character.LiveType).FirstOrDefault();
            animationRequest.animationType = VAnimationType.AddConsumable;
            animationRequest.consumableIDs = new List<uint>(){ _consumable.ConfigId };
            animationRequest.returnable = true;
            base.ApplyEffect(character, messagedict, animationRequest);
        }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            _character.ConsumableManager.AddConsumable(_consumable);
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