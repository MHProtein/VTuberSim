using System.Collections.Generic;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Character.Attributes;

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

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            if (character.AttributeManager.TryGetAttribute(AttributeName, out var attribute))
            {
                var abilityAttribute = attribute as VAbilityAttribute;
                if (abilityAttribute is not null) abilityAttribute.AddAbility(_value.Value, _shouldUseEfficiency);
            }
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
    }
}