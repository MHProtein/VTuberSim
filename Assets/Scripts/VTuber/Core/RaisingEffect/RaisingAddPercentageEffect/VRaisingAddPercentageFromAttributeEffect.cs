using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Character.Attributes;
using VTuber.Core.UI;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddPercentageFromAttributeEffect : VRaisingEffect, IAttributeEffect
    {
        private readonly VUpgradableValue<float> _percentage;
        public string attributeNameToAdd;
        public string attributeNameToBeAdded;

        public VRaisingAddPercentageFromAttributeEffect(
            VRaisingAddPercentageFromAttributeEffectConfiguration configuration, string parameter,
            string upgradedParameter) : base(configuration)
        {
            attributeNameToAdd = configuration.attributeNameToAdd;
            attributeNameToBeAdded = configuration.attributeNameToBeAdded;
            _percentage = new VUpgradableValue<float>(float.Parse(parameter), float.Parse(upgradedParameter));
        }

        public string AttributeName => attributeNameToBeAdded;

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.AttributeManager.TryGetAttribute(attributeNameToAdd, out var attributeToAdd);
            if (character.AttributeManager.TryGetAttribute(attributeNameToBeAdded, out var attribute))
            {
                if (attributeNameToBeAdded.Contains("Ability"))
                {
                    var abilityAttribute = attribute as VAbilityAttribute;
                    if (abilityAttribute is not null)
                        abilityAttribute.AddAbility(VMathUtils.FloatToInt(_percentage.Value * attributeToAdd.Value),
                            false);
                }
                else
                {
                    attribute.AddTo(VMathUtils.FloatToInt(_percentage.Value * attributeToAdd.Value), true);
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

    public class VRaisingAddPercentageFromAttributeEffectConfiguration : VRaisingEffectConfiguration
    {
        public string attributeNameToAdd;
        public string attributeNameToBeAdded;

        public VRaisingAddPercentageFromAttributeEffectConfiguration(CellRange row) : base(row)
        {
            var parameters = row.Columns[VRaisingEffectHeaderIndex.Param].Value.Split(',');
            attributeNameToAdd = parameters[0];
            attributeNameToBeAdded = parameters[1];
        }

        public override VRaisingEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VRaisingAddPercentageFromAttributeEffect(this, parameter, upgradedParameter);
        }
    }
}