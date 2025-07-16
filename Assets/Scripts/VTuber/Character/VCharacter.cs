using System.Collections.Generic;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;

namespace VTuber.Character
{
    public class VCharacter
    {
        public string Name { get; set; }
        
        public VCharacterAttributeManager AttributeManager { get; private set; }

        private VCharacterConfiguration _characterConfig;
        
        public VCharacter(VCharacterConfiguration characterConfig)
        {
            // _characterConfig = characterConfig;
            // AttributeManager = new VCharacterAttributeManager();
            // AttributeManager.AddAttribute("CAStamina",
            //     new VCharacterAttribute(characterConfig.staminaConfiguration, 
            //         characterConfig.staminaInitialValue, 
            //         VRaisingEventKey.Default, 
            //         characterConfig.staminaMaxValue, 
            //         characterConfig.staminaMinValue));
            //
            // AttributeManager.AddAttribute("CAPressure",
            //     new VCharacterAttribute(characterConfig.pressureConfiguration, 
            //         characterConfig.pressureInitialValue, 
            //         VRaisingEventKey.Default, 
            //         characterConfig.pressureMaxValue, 
            //         characterConfig.pressureMinValue));
            //
            // AttributeManager.AddAttribute("CASingingAbility", 
            //     new VCharacterAttribute(characterConfig.singingAbilityConfiguration,
            //         characterConfig.singingAbilityInitialValue, 
            //         VRaisingEventKey.Default, 
            //         characterConfig.singingAbilityMaxValue, 
            //         characterConfig.singingAbilityMinValue));
            //
            // AttributeManager.AddAttribute("CAGamingAbility", 
            //     new VCharacterAttribute(characterConfig.gamingAbilityConfiguration,
            //         characterConfig.gamingAbilityInitialValue, 
            //         VRaisingEventKey.Default, 
            //         characterConfig.gamingAbilityMaxValue, 
            //         characterConfig.gamingAbilityMinValue));
            //
            // AttributeManager.AddAttribute("CAChattingAbility",
            //     new VCharacterAttribute(characterConfig.chattingAbilityConfiguration,
            //         characterConfig.chattingAbilityInitialValue, 
            //         VRaisingEventKey.Default, 
            //         characterConfig.chattingAbilityMaxValue, 
            //         characterConfig.chattingAbilityMinValue));
            //
            // AttributeManager.AddAttribute("CASingingAbilityConversionRatio",
            //     new VCharacterAttribute(characterConfig.singingAbilityConversionRatioConfiguration,
            //         characterConfig.singingAbilityConversionRatioInitialValue,
            //         VRaisingEventKey.Default,
            //         characterConfig.singingAbilityConversionRatioMaxValue,
            //         characterConfig.singingAbilityConversionRatioMinValue)
            //     );
            //
            // AttributeManager.AddAttribute("CAGamingAbilityConversionRatio",
            //     new VCharacterAttribute(characterConfig.gamingAbilityConversionRatioConfiguration,
            //         characterConfig.gamingAbilityConversionRatioInitialValue,
            //         VRaisingEventKey.Default,
            //         characterConfig.gamingAbilityConversionRatioMaxValue,
            //         characterConfig.gamingAbilityConversionRatioMinValue));
            //         
            // AttributeManager.AddAttribute("CAChattingAbilityConversionRatio",
            //     new VCharacterAttribute(characterConfig.chattingAbilityConversionRatioConfiguration,
            //         characterConfig.chattingAbilityConversionRatioInitialValue,
            //         VRaisingEventKey.Default,
            //         characterConfig.chattingAbilityConversionRatioMaxValue,
            //         characterConfig.chattingAbilityConversionRatioMinValue));
            //
            // AttributeManager.AddAttribute("CASingingAbilityGainEfficiency",
            //     new VCharacterAttribute(characterConfig.singingAbilityGainEfficiencyConfiguration,
            //         characterConfig.singingAbilityGainEfficiencyInitialValue,
            //         VRaisingEventKey.Default,
            //         characterConfig.singingAbilityGainEfficiencyMaxValue,
            //         characterConfig.singingAbilityGainEfficiencyMinValue));
            //
            // AttributeManager.AddAttribute("CAGamingAbilityGainEfficiency",
            //     new VCharacterAttribute(characterConfig.gamingAbilityGainEfficiencyConfiguration,
            //         characterConfig.gamingAbilityGainEfficiencyInitialValue,
            //         VRaisingEventKey.Default,
            //         characterConfig.gamingAbilityGainEfficiencyMaxValue,
            //         characterConfig.gamingAbilityGainEfficiencyMinValue));
            //
            // AttributeManager.AddAttribute("CAChattingAbilityGainEfficiency",
            //     new VCharacterAttribute(characterConfig.chattingAbilityGainEfficiencyConfiguration,
            //         characterConfig.chattingAbilityGainEfficiencyInitialValue,
            //         VRaisingEventKey.Default,
            //         characterConfig.chattingAbilityGainEfficiencyMaxValue,
            //         characterConfig.chattingAbilityGainEfficiencyMinValue));
            //
            // AttributeManager.AddAttribute("CAFollowerCount", 
            //     new VCharacterAttribute(characterConfig.followerCountConfiguration,
            //         characterConfig.followerCountInitialValue, 
            //         VRaisingEventKey.Default, 
            //         characterConfig.followerCountMaxValue, 
            //         characterConfig.followerCountMinValue));
            //
            // AttributeManager.AddAttribute("CAMembershipCount",
            //     new VCharacterAttribute(characterConfig.membershipCountConfiguration,
            //         characterConfig.membershipCountInitialValue, 
            //         VRaisingEventKey.Default, 
            //         characterConfig.membershipCountMaxValue, 
            //         characterConfig.membershipCountMinValue));
            //
            // AttributeManager.AddAttribute("CAFollowerToViewerRatio",
            //     new VCharacterAttribute(characterConfig.followerToViewerRatioConfiguration,
            //         characterConfig.followerToViewerRatioInitialValue, 
            //         VRaisingEventKey.Default, 
            //         characterConfig.followerToViewerRatioMaxValue, 
            //         characterConfig.followerToViewerRatioMinValue));
            //
            // AttributeManager.AddAttribute("CAMoney",
            //     new VCharacterAttribute(characterConfig.moneyConfiguration,
            //         characterConfig.moneyInitialValue, 
            //         VRaisingEventKey.Default, 
            //         characterConfig.moneyMaxValue, 
            //         characterConfig.moneyMinValue));
        }
        
        
    }
}













