using System.Collections.Generic;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.UI;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAttributeMaxValueEffect : VRaisingEffect, IAttributeEffect
    {
        private readonly VUpgradableValue<int> _value;

        public VRaisingAddAttributeMaxValueEffect(VRaisingAddAttributeMaxValueEffectConfiguration configuration,
            string parameter, string upgradedParameter) : base(configuration)
        {
            AttributeName = configuration.attributeName;
            _value = new VUpgradableValue<int>(int.Parse(parameter.Trim()), int.Parse(upgradedParameter.Trim()));
        }

        public string AttributeName { get; }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            if (animationRequest is not null)
            {
                animationRequest.attributeIcon = VUIUtils.Instance.GetAttributeIcon(AttributeName);
                animationRequest.isMaxValue = true;
                animationRequest.isPercentage = character.AttributeManager.Attributes[AttributeName].IsPercentage;
            }
            base.ApplyEffect(character, messagedict, animationRequest);
        }
        
        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            if (character.AttributeManager.TryGetAttribute(AttributeName, out var attribute))
                attribute.AddMaxValue(_value.Value);
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
            return _value.Value;
        }
    }
}