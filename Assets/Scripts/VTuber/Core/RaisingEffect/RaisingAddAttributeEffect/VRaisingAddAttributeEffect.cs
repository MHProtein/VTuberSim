using System.Collections.Generic;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.UI;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAttributeEffect : VRaisingEffect, IAttributeEffect
    {
        private readonly VUpgradableValue<int> _value;

        public VRaisingAddAttributeEffect(VRaisingAddAttributeEffectConfiguration configuration, int value,
            int upgradedValue) : base(configuration)
        {
            AttributeName = configuration.AbilityName;
            _value = new VUpgradableValue<int>(value, upgradedValue);
        }

        public string AttributeName { get; }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            if (character.AttributeManager.TryGetAttribute(AttributeName, out var attribute))
            {
                attribute.AddTo(_value.Value, true);
                VDebug.Log("Added " + _value + " To " + AttributeName);
            }
        }
        
        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            if (animationRequest is not null)
            {
                animationRequest.attributeIcon = VUIUtils.Instance.GetAttributeIcon(AttributeName);
                animationRequest.isPercentage = character.AttributeManager.Attributes[AttributeName].IsPercentage;

                if (AttributeName == "CAPressure")
                {
                    animationRequest.instigatorType = VInstigatorType.Ignore;
                    animationRequest.animationType = VAnimationType.Pressure;
                    var attribute = character.AttributeManager.Attributes[AttributeName];
                    animationRequest.currentPressureLevel = attribute.Value;
                    animationRequest.nextPressureLevel = attribute.PreviewAddTo(_value.Value, true);
                    if (animationRequest.currentPressureLevel == animationRequest.nextPressureLevel)
                        return;
                }
            }
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
            return _value.Value;
        }
    }
}