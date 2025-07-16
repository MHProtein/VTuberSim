using System.Collections.Generic;
using VTuber.Character.Attribute;

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
            //     new VCharacterAttribute(characterConfig.staminaConfiguration));
            // AttributeManager.AddAttribute("CAPressure",
            //     new VCharacterAttribute(characterConfig.pressureConfiguration));
            //
            // AttributeManager.AddAttribute("CASingingAbility",
            //     new VCharacterAttribute(characterConfig.singingAbilityConfiguration));
            // AttributeManager.AddAttribute("CAGamingAbility", 
            //     new VCharacterAttribute(characterConfig.gamingAbilityConfiguration));
            // AttributeManager.AddAttribute("CAChattingAbility",
            //     new VCharacterAttribute(characterConfig.chattingAbilityConfiguration));
            //
            // AttributeManager.AddAttribute("CASingingAbilityConversionRatio",
            //     new VCharacterAttribute(characterConfig.singingAbilityConversionRatioConfiguration));
            // AttributeManager.AddAttribute("CAGamingAbilityConversionRatio",
            //     new VCharacterAttribute(characterConfig.gamingAbilityConversionRatioConfiguration));
            // AttributeManager.AddAttribute("CAChattingAbilityConversionRatio",
            //     new VCharacterAttribute(characterConfig.chattingAbilityConversionRatioConfiguration));
            //
            // AttributeManager.AddAttribute("CASingingAbilityGainEfficiency",
            //     new VCharacterAttribute(characterConfig.singingAbilityGainEfficiencyConfiguration));
            // AttributeManager.AddAttribute("CAGamingAbilityGainEfficiency",
            //     new VCharacterAttribute(characterConfig.gamingAbilityGainEfficiencyConfiguration));
            // AttributeManager.AddAttribute("CAChattingAbilityGainEfficiency",
            //     new VCharacterAttribute(characterConfig.chattingAbilityGainEfficiencyConfiguration));
            //
            // AttributeManager.AddAttribute("CAFollowerCount", 
            //     new VCharacterAttribute(characterConfig.followerCountConfiguration));
            // AttributeManager.AddAttribute("CAMembershipCount",
            //     new VCharacterAttribute(characterConfig.membershipCountConfiguration));
            // AttributeManager.AddAttribute("CAFollowerToViewerRatio",
            //     new VCharacterAttribute(characterConfig.followerToViewerRatioConfiguration));
            //
            // AttributeManager.AddAttribute("CAMoney",
            //     new VCharacterAttribute(characterConfig.moneyConfiguration));
        }
        
        
    }
}













