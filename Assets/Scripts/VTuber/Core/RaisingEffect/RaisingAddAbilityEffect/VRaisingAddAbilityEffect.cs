using System.Collections.Generic;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Character.Attributes;
using VTuber.Core.UI;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public interface IAttributeEffect
    {
        string AttributeName { get; }
    }

    public class VRaisingAddAbilityEffect : VRaisingEffect, IAttributeEffect
    {
        private readonly bool _shouldUseEfficiency;
        private readonly VUpgradableValue<int> _value;

        public VRaisingAddAbilityEffect(VRaisingAddAbilityEffectConfiguration configuration, int value,
            int upgradedValue) : base(configuration)
        {
            AttributeName = configuration.AbilityName;
            _shouldUseEfficiency = configuration.ShouldUseEfficiency;
            _value = new VUpgradableValue<int>(value, upgradedValue);
        }

        public string AttributeName { get; }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            if (character.AttributeManager.TryGetAttribute(AttributeName, out var attribute))
            {
                var abilityAttribute = attribute as VAbilityAttribute;
                if (abilityAttribute is not null) abilityAttribute.AddAbility(_value.Value, _shouldUseEfficiency);
            }
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            if(animationRequest is not null)
                animationRequest.attributeIcon = VUIUtils.Instance.GetAttributeIcon(AttributeName);
            base.ApplyEffect(character, messagedict, animationRequest);
        }

        public override void Upgrade()
        {
            _value.Upgrade();
        }

        public override void DownGrade()
        {
            _value.Downgrade();
        }

        public override string GetParameter()
        {
            return _value.Value.ToString();
        }
        
        protected override int GetPreviewValue(VCharacter character)
        {          
            var previewValue = 0;
            if (character.AttributeManager.TryGetAttribute(AttributeName, out var attribute))
            {
                var abilityAttribute = attribute as VAbilityAttribute;
                if (abilityAttribute is not null) previewValue = abilityAttribute.PreviewAddTo(_value.Value) - abilityAttribute.Value;
            }

            return previewValue;
        }
    }
}