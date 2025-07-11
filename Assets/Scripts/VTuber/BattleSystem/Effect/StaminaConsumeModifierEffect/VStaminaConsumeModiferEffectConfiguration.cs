using Sirenix.OdinInspector;
using UnityEngine;

namespace VTuber.BattleSystem.Effect.StaminaConsumeModifierEffect
{
    public enum VStaminaConsumeModifierType
    {
        Rate,
        Points
    }
    public class VStaminaConsumeModiferEffectConfiguration : VEffectConfiguration
    {
        public VStaminaConsumeModifierType modifierType;
        
        [Tooltip("Rate += Rate + Delta")]
        [ShowIf("@modifierType == VStaminaConsumeModifierType.Rate")]
        public float deltaRate;
        
        [Tooltip("Points += Points + Delta")]
        [ShowIf("@modifierType == VStaminaConsumeModifierType.Points")]
        public int deltaPoints;
        
        public override VEffect CreateEffect()
        {
            return new VStaminaConsumeModiferEffect(this);
        }
    }
}