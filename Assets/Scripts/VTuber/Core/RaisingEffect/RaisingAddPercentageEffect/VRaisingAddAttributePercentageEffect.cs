using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Character.Attributes;
using VTuber.Core.UI;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAttributePercentageEffect : VRaisingEffect, IAttributeEffect
    {
        private readonly VUpgradableValue<float> _percentage;

        public VRaisingAddAttributePercentageEffect(VRaisingAddAttributePercentageEffectConfiguration configuration,
            string parameter, string upgradedParameter) : base(configuration)
        {
            AttributeName = configuration.abilityName;
            _percentage = new VUpgradableValue<float>(float.Parse(parameter),
                float.Parse(upgradedParameter));
        }

        public string AttributeName { get; }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            if (character.AttributeManager.TryGetAttribute(AttributeName, out var attribute))
            {
                if (AttributeName.Contains("Ability"))
                {
                    var abilityAttribute = attribute as VAbilityAttribute;
                    if (abilityAttribute is not null)
                        abilityAttribute.AddAbility(VMathUtils.FloatToInt(_percentage.Value * abilityAttribute.Value),
                            false);
                }
                else
                {
                    attribute.AddTo(VMathUtils.FloatToInt(_percentage.Value * attribute.Value), true);
                }
            }
        }
        
        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            animationRequest.attributeIcon = VUIUtils.Instance.GetAttributeIcon(AttributeName);
            base.ApplyEffect(character, messagedict, animationRequest);
        }

        public override void Upgrade()
        {
            _percentage.Upgrade();
        }

        public override void DownGrade()
        {
            _percentage.Downgrade();
        }

        public override string GetParameter()
        {
            return VMathUtils.FloatToInt(_percentage.Value * 100) + "%";
        }

        protected override int GetPreviewValue(VCharacter character)
        {
            var previewValue = 0;
            if (character.AttributeManager.TryGetAttribute(AttributeName, out var attribute))
            {
                if (AttributeName.Contains("Ability"))
                {
                    var abilityAttribute = attribute as VAbilityAttribute;
                    if (abilityAttribute is not null)
                        previewValue = abilityAttribute.PreviewAddTo(VMathUtils.FloatToInt(_percentage.Value * abilityAttribute.Value)) - abilityAttribute.Value;
                }
                else
                {
                    previewValue = attribute.PreviewAddTo(VMathUtils.FloatToInt(_percentage.Value * attribute.Value)) - attribute.Value;
                }
            }
            return previewValue;
        }
    }

    public class VRaisingAddAttributePercentageEffectConfiguration : VRaisingEffectConfiguration
    {
        public string abilityName;

        public VRaisingAddAttributePercentageEffectConfiguration(CellRange row) : base(row)
        {
            abilityName = row.Columns[VRaisingEffectHeaderIndex.Param].Value;
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingAddAttributePercentageEffect(this, parameter, upgradedParameter);
        }
    }
}