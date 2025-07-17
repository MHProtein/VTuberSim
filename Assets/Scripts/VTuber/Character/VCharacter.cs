using System.Collections.Generic;
using VTuber.Character.Attribute;
using VTuber.Character.Attributes;
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
            _characterConfig = characterConfig;
            AttributeManager = new VCharacterAttributeManager();
            AttributeManager.AddAttribute("CAStamina",
                new VStaminaAttribute(characterConfig.staminaConfiguration, 
                    characterConfig.staminaInitialValue, 
                    VRaisingEventKey.OnStaminaChanged, 
                    characterConfig.staminaMaxValue, 
                    characterConfig.staminaMinValue));
            
            AttributeManager.AddAttribute("CAPressure",
                new VPressureAttribute(characterConfig.pressureConfiguration, 
                    characterConfig.pressureBuffs,
                    characterConfig.pressureInitialValue, 
                    VRaisingEventKey.OnPressureChanged, 
                    characterConfig.pressureMaxValue, 
                    characterConfig.pressureMinValue));
            
            AttributeManager.AddAttribute("CASingingAbility", 
                new VCharacterAttribute(characterConfig.singingAbilityConfiguration,
                    characterConfig.singingAbilityInitialValue, 
                    VRaisingEventKey.OnSingingAbilityChanged, 
                    characterConfig.singingAbilityMaxValue, 
                    characterConfig.singingAbilityMinValue));
            
            AttributeManager.AddAttribute("CAGamingAbility", 
                new VCharacterAttribute(characterConfig.gamingAbilityConfiguration,
                    characterConfig.gamingAbilityInitialValue, 
                    VRaisingEventKey.OnGamingAbilityChanged, 
                    characterConfig.gamingAbilityMaxValue, 
                    characterConfig.gamingAbilityMinValue));
            
            AttributeManager.AddAttribute("CAChattingAbility",
                new VCharacterAttribute(characterConfig.chattingAbilityConfiguration,
                    characterConfig.chattingAbilityInitialValue, 
                    VRaisingEventKey.OnChattingAbilityChanged, 
                    characterConfig.chattingAbilityMaxValue, 
                    characterConfig.chattingAbilityMinValue));
            
            AttributeManager.AddAttribute("CASingingAbilityConversionRatio",
                new VCharacterAttribute(characterConfig.singingAbilityConversionRatioConfiguration,
                    characterConfig.singingAbilityConversionRatioInitialValue,
                    VRaisingEventKey.OnSingingAbilityConversionRatioChanged,
                    characterConfig.singingAbilityConversionRatioMaxValue,
                    characterConfig.singingAbilityConversionRatioMinValue)
                );
            
            AttributeManager.AddAttribute("CAGamingAbilityConversionRatio",
                new VCharacterAttribute(characterConfig.gamingAbilityConversionRatioConfiguration,
                    characterConfig.gamingAbilityConversionRatioInitialValue,
                    VRaisingEventKey.OnGamingAbilityConversionRatioChanged,
                    characterConfig.gamingAbilityConversionRatioMaxValue,
                    characterConfig.gamingAbilityConversionRatioMinValue));
                    
            AttributeManager.AddAttribute("CAChattingAbilityConversionRatio",
                new VCharacterAttribute(characterConfig.chattingAbilityConversionRatioConfiguration,
                    characterConfig.chattingAbilityConversionRatioInitialValue,
                    VRaisingEventKey.OnChattingAbilityConversionRatioChanged,
                    characterConfig.chattingAbilityConversionRatioMaxValue,
                    characterConfig.chattingAbilityConversionRatioMinValue));
            
            AttributeManager.AddAttribute("CASingingAbilityGainEfficiency",
                new VCharacterAttribute(characterConfig.singingAbilityGainEfficiencyConfiguration,
                    characterConfig.singingAbilityGainEfficiencyInitialValue,
                    VRaisingEventKey.OnSingingAbilityGainEfficiencyChanged,
                    characterConfig.singingAbilityGainEfficiencyMaxValue,
                    characterConfig.singingAbilityGainEfficiencyMinValue));
            
            AttributeManager.AddAttribute("CAGamingAbilityGainEfficiency",
                new VCharacterAttribute(characterConfig.gamingAbilityGainEfficiencyConfiguration,
                    characterConfig.gamingAbilityGainEfficiencyInitialValue,
                    VRaisingEventKey.OnGamingAbilityGainEfficiencyChanged,
                    characterConfig.gamingAbilityGainEfficiencyMaxValue,
                    characterConfig.gamingAbilityGainEfficiencyMinValue));
            
            AttributeManager.AddAttribute("CAChattingAbilityGainEfficiency",
                new VCharacterAttribute(characterConfig.chattingAbilityGainEfficiencyConfiguration,
                    characterConfig.chattingAbilityGainEfficiencyInitialValue,
                    VRaisingEventKey.OnChattingAbilityGainEfficiencyChanged,
                    characterConfig.chattingAbilityGainEfficiencyMaxValue,
                    characterConfig.chattingAbilityGainEfficiencyMinValue));
            
            AttributeManager.AddAttribute("CAFollowerCount", 
                new VCharacterAttribute(characterConfig.followerCountConfiguration,
                    characterConfig.followerCountInitialValue, 
                    VRaisingEventKey.OnFollowerCountChanged, 
                    characterConfig.followerCountMaxValue, 
                    characterConfig.followerCountMinValue));
            
            AttributeManager.AddAttribute("CAMembershipCount",
                new VCharacterAttribute(characterConfig.membershipCountConfiguration,
                    characterConfig.membershipCountInitialValue, 
                    VRaisingEventKey.OnMemberCountChanged, 
                    characterConfig.membershipCountMaxValue, 
                    characterConfig.membershipCountMinValue));
            
            AttributeManager.AddAttribute("CAFollowerToViewerRatio",
                new VCharacterAttribute(characterConfig.followerToViewerRatioConfiguration,
                    characterConfig.followerToViewerRatioInitialValue, 
                    VRaisingEventKey.OnFollowerToViewerRatioChanged, 
                    characterConfig.followerToViewerRatioMaxValue, 
                    characterConfig.followerToViewerRatioMinValue));
            
            AttributeManager.AddAttribute("CAMoney",
                new VCharacterAttribute(characterConfig.moneyConfiguration,
                    characterConfig.moneyInitialValue, 
                    VRaisingEventKey.OnMoneyChanged, 
                    characterConfig.moneyMaxValue, 
                    characterConfig.moneyMinValue));
        }
        
        
    }
}













