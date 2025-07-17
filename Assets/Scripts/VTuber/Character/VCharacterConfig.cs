using System.Collections.Generic;
using VTuber.Character.Attribute;
using VTuber.Core.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VTuber.Character
{
    public class VCharacterConfiguration : VScriptableObject
    {
        
        [HorizontalGroup("StaminaGroup", MarginLeft = 0)]
        [LabelText("Init")]
        public int staminaInitialValue;

        [HorizontalGroup("StaminaGroup")]
        [LabelText("Min")]
        public int staminaMinValue;

        [LabelText("Max")]
        public int staminaMaxValue;
        
        [PropertySpace(10)]
        [LabelText("Config")]
        public VCharacterAttributeConfiguration staminaConfiguration;
        // public int pressureInitialValue;
        // public int pressureMinValue;
        // public int pressureMaxValue;
        // public VCharacterAttributeConfiguration pressureConfiguration;
        //
        // public int singingAbilityInitialValue;
        // public int singingAbilityMinValue;
        // public int singingAbilityMaxValue;
        // public VCharacterAttributeConfiguration singingAbilityConfiguration;
        //
        // public int gamingAbilityInitialValue;
        // public int gamingAbilityMinValue;
        // public int gamingAbilityMaxValue;
        // public VCharacterAttributeConfiguration gamingAbilityConfiguration;
        //
        // public int chattingAbilityInitialValue;
        // public int chattingAbilityMinValue;
        // public int chattingAbilityMaxValue;
        // public VCharacterAttributeConfiguration chattingAbilityConfiguration;
        //
        // public int singingAbilityConversionRatioInitialValue;
        // public int singingAbilityConversionRatioMinValue;
        // public int singingAbilityConversionRatioMaxValue;
        // public VCharacterAttributeConfiguration singingAbilityConversionRatioConfiguration;
        //
        // public int gamingAbilityConversionRatioInitialValue;
        // public int gamingAbilityConversionRatioMinValue;
        // public int gamingAbilityConversionRatioMaxValue;
        // public VCharacterAttributeConfiguration gamingAbilityConversionRatioConfiguration;
        //
        // public int chattingAbilityConversionRatioInitialValue;
        // public int chattingAbilityConversionRatioMinValue;
        // public int chattingAbilityConversionRatioMaxValue;
        // public VCharacterAttributeConfiguration chattingAbilityConversionRatioConfiguration;
        //
        //
        // public int singingAbilityGainEfficiencyInitialValue;
        // public int singingAbilityGainEfficiencyMinValue;
        // public int singingAbilityGainEfficiencyMaxValue;
        // public VCharacterAttributeConfiguration singingAbilityGainEfficiencyConfiguration;
        //
        // public int gamingAbilityGainEfficiencyInitialValue;
        // public int gamingAbilityGainEfficiencyMinValue;
        // public int gamingAbilityGainEfficiencyMaxValue;
        // public VCharacterAttributeConfiguration gamingAbilityGainEfficiencyConfiguration;
        //
        // public int chattingAbilityGainEfficiencyInitialValue;
        // public int chattingAbilityGainEfficiencyMinValue;
        // public int chattingAbilityGainEfficiencyMaxValue;
        // public VCharacterAttributeConfiguration chattingAbilityGainEfficiencyConfiguration;
        //
        // public int followerCountInitialValue;
        // public int followerCountMinValue;
        // public int followerCountMaxValue;
        // public VCharacterAttributeConfiguration followerCountConfiguration;
        //
        // public int membershipCountInitialValue;
        // public int membershipCountMinValue;
        // public int membershipCountMaxValue;
        // public VCharacterAttributeConfiguration membershipCountConfiguration;
        //
        // public int followerToViewerRatioInitialValue;
        // public int followerToViewerRatioMinValue;
        // public int followerToViewerRatioMaxValue;
        // public VCharacterAttributeConfiguration followerToViewerRatioConfiguration;
        //
        // public int moneyInitialValue;
        // public int moneyMinValue;
        // public int moneyMaxValue;
        // public VCharacterAttributeConfiguration moneyConfiguration;
    }
}