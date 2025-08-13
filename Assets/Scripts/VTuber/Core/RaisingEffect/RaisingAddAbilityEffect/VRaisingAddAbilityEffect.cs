using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Character.Attributes;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAbilityEffect : VRaisingEffect
    {
        private readonly string _attributeName;
        private readonly VUpgradableValue<int> _value;
        private readonly bool _shouldUseEfficiency;
        public VRaisingAddAbilityEffect(VRaisingAddAbilityEffectConfiguration configuration, int value, int upgradedValue) : base(configuration)
        {
            _attributeName = configuration.AbilityName;
            _shouldUseEfficiency = configuration.ShouldUseEfficiency;
            _value = new VUpgradableValue<int>(value, upgradedValue);
        }

        public override void ApplyEffect(VCharacter character)
        {
            if(character.AttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                var abilityAttribute = attribute as VAbilityAttribute;
                if (abilityAttribute is not null)
                {
                    abilityAttribute.AddAbility(_value.Value, _shouldUseEfficiency);
                }
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
    }
}