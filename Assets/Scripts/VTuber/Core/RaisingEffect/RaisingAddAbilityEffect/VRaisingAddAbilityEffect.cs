using VTuber.Character;
using VTuber.Character.Attributes;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core.RaisingEffect.RaisingAddAbilityEffect
{
    public class VRaisingAddAbilityEffect : VRaisingEffect
    {
        public string attributeName;
        public int value;
        public bool shouldIgnoreEfficiency;
        public VRaisingAddAbilityEffect(VRaisingEffectConfiguration configuration) : base(configuration)
        {
            
        }

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);
            if(character.AttributeManager.TryGetAttribute(attributeName, out var attribute))
            {
                var abilityAttribute = attribute as VAbilityAttribute;
                if (abilityAttribute is not null)
                {
                    abilityAttribute.AddAbility(value, shouldIgnoreEfficiency);
                }
                
            }
        }
    }
}