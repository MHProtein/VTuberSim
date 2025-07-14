using System;
using CsvHelper;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VTuber.BattleSystem.Effect
{
    public enum VStaminaCostModifierType
    {
        Rate,
        Points
    }
    public class VStaminaCostModiferEffectConfiguration : VEffectConfiguration
    {
        public VStaminaCostModifierType modifierType;
        
        [Tooltip("Rate += Rate + Delta")]
        [ShowIf("@modifierType == VStaminaConsumeModifierType.Rate")]
        public float deltaRate;
        
        [Tooltip("Points += Points + Delta")]
        [ShowIf("@modifierType == VStaminaConsumeModifierType.Points")]
        public int deltaPoints;
        
        public VStaminaCostModiferEffectConfiguration(CsvReader csv) : base(csv)
        {
            modifierType = Enum.Parse<VStaminaCostModifierType>(csv.GetField<string>("StaminaModifyType"));
            deltaRate = csv.GetField<float>("StaminaDelta");
            deltaPoints = csv.GetField<int>("StaminaDelta");
        }

        public override VEffect CreateEffect()
        {
            return new VStaminaCostModiferEffect(this);
        }
    }
}