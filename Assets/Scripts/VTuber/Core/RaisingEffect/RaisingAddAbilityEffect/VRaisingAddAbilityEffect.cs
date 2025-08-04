using VTuber.Character;
using VTuber.Character.Attributes;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAbilityEffect : VRaisingEffect
    {
        private readonly string _attributeName;
        private readonly int _value;
        private readonly bool _shouldUseEfficiency;
        public VRaisingAddAbilityEffect(VRaisingAddAbilityEffectConfiguration configuration, int value) : base(configuration)
        {
            _attributeName = configuration.AbilityName;
            _shouldUseEfficiency = configuration.ShouldUseEfficiency;
            _value = value;
        }

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);
            if(character.AttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                var abilityAttribute = attribute as VAbilityAttribute;
                if (abilityAttribute is not null)
                {
                    abilityAttribute.AddAbility(_value, _shouldUseEfficiency);
                }
            }
        }
    }
}