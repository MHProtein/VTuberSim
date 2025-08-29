using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Character.Attributes;
using VTuber.Core.UI;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAttributePercentageEffect : VRaisingEffect, IAttributeEffect
    {
        public string AttributeName => _attributeName;
        private readonly string _attributeName;
        private readonly VUpgradableValue<float> _percentage;
        public VRaisingAddAttributePercentageEffect(VRaisingAddAttributePercentageEffectConfiguration configuration,
            string parameter, string upgradedParameter) : base(configuration)
        {
            _attributeName = configuration.abilityName;
            _percentage = new VUpgradableValue<float>(float.Parse(parameter),
                float.Parse(upgradedParameter));
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            if(character.AttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                if (_attributeName.Contains("Ability"))
                {
                    var abilityAttribute = attribute as VAbilityAttribute;
                    if (abilityAttribute is not null)
                    {
                        abilityAttribute.AddAbility(VMathUtils.FloatToInt(_percentage.Value * abilityAttribute.Value),
                            false);
                    }
                }               
                else
                {

                    attribute.AddTo(VMathUtils.FloatToInt(_percentage.Value * attribute.Value));
                }
            }
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