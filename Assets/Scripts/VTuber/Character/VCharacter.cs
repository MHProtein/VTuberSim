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
                    characterConfig.staminaMaxValue == -1 ? int.MaxValue : characterConfig.staminaMaxValue,
                    characterConfig.staminaMinValue));
            
            AttributeManager.AddAttribute("CAPressure",
                new VPressureAttribute(characterConfig.pressureConfiguration, 
                    characterConfig.pressureBuffs,
                    characterConfig.pressureInitialValue, 
                    VRaisingEventKey.OnPressureChanged, 
                    characterConfig.pressureMaxValue == -1 ? int.MaxValue : characterConfig.pressureMaxValue,
                    characterConfig.pressureMinValue));
            
            AttributeManager.AddAttribute("CASingingAbility", 
                new VAbilityAttribute(characterConfig.singingAbilityConfiguration,      
                    characterConfig.singingAbilityColor,
                    characterConfig.singingAbilityInitialValue, 
                    VRaisingEventKey.OnSingingAbilityChanged, 
                    characterConfig.singingAbilityMaxValue == -1 ? int.MaxValue : characterConfig.singingAbilityMaxValue,
                    characterConfig.singingAbilityMinValue));
            
            AttributeManager.AddAttribute("CAGamingAbility", 
                new VAbilityAttribute(characterConfig.gamingAbilityConfiguration,
                    characterConfig.gamingAbilityColor,
                    characterConfig.gamingAbilityInitialValue, 
                    VRaisingEventKey.OnGamingAbilityChanged, 
                    characterConfig.gamingAbilityMaxValue == -1 ? int.MaxValue : characterConfig.gamingAbilityMaxValue,
                    characterConfig.gamingAbilityMinValue));
            
            AttributeManager.AddAttribute("CAChattingAbility",
                new VAbilityAttribute(characterConfig.chattingAbilityConfiguration,
                    characterConfig.chattingAbilityColor,
                    characterConfig.chattingAbilityInitialValue, 
                    VRaisingEventKey.OnChattingAbilityChanged, 
                    characterConfig.chattingAbilityMaxValue == -1 ? int.MaxValue : characterConfig.chattingAbilityMaxValue,
                    characterConfig.chattingAbilityMinValue));
            
            AttributeManager.AddAttribute("CASingingAbilityConversionRatio",
                new VConversionRatioAttribute(characterConfig.singingAbilityConversionRatioConfiguration,
                    characterConfig.singingAbilityConversionRatioInitialValue,
                    VRaisingEventKey.OnSingingAbilityConversionRatioChanged,
                    characterConfig.singingAbilityConversionRatioMaxValue == -1 ? int.MaxValue : characterConfig.singingAbilityConversionRatioMaxValue,
                    characterConfig.singingAbilityConversionRatioMinValue)
                );
            
            AttributeManager.AddAttribute("CAGamingAbilityConversionRatio",
                new VConversionRatioAttribute(characterConfig.gamingAbilityConversionRatioConfiguration,
                    characterConfig.gamingAbilityConversionRatioInitialValue,
                    VRaisingEventKey.OnGamingAbilityConversionRatioChanged,
                    characterConfig.gamingAbilityConversionRatioMaxValue == -1 ? int.MaxValue : characterConfig.gamingAbilityConversionRatioMaxValue,
                    characterConfig.gamingAbilityConversionRatioMinValue));
                    
            AttributeManager.AddAttribute("CAChattingAbilityConversionRatio",
                new VConversionRatioAttribute(characterConfig.chattingAbilityConversionRatioConfiguration,
                    characterConfig.chattingAbilityConversionRatioInitialValue,
                    VRaisingEventKey.OnChattingAbilityConversionRatioChanged,
                    characterConfig.chattingAbilityConversionRatioMaxValue == -1 ? int.MaxValue : characterConfig.chattingAbilityConversionRatioMaxValue,
                    characterConfig.chattingAbilityConversionRatioMinValue));
            
            AttributeManager.AddAttribute("CASingingAbilityGainEfficiency",
                new VAbilityGainEfficiencyAttribute(characterConfig.singingAbilityGainEfficiencyConfiguration,
                    characterConfig.singingAbilityGainEfficiencyInitialValue,
                    VRaisingEventKey.OnSingingAbilityGainEfficiencyChanged,
                    characterConfig.singingAbilityGainEfficiencyMaxValue == -1 ? int.MaxValue : characterConfig.singingAbilityGainEfficiencyMaxValue,
                    characterConfig.singingAbilityGainEfficiencyMinValue));
            
            AttributeManager.AddAttribute("CAGamingAbilityGainEfficiency",
                new VAbilityGainEfficiencyAttribute(characterConfig.gamingAbilityGainEfficiencyConfiguration,
                    characterConfig.gamingAbilityGainEfficiencyInitialValue,
                    VRaisingEventKey.OnGamingAbilityGainEfficiencyChanged,
                    characterConfig.gamingAbilityGainEfficiencyMaxValue == -1 ? int.MaxValue : characterConfig.gamingAbilityGainEfficiencyMaxValue,
                    characterConfig.gamingAbilityGainEfficiencyMinValue));
            
            AttributeManager.AddAttribute("CAChattingAbilityGainEfficiency",
                new VAbilityGainEfficiencyAttribute(characterConfig.chattingAbilityGainEfficiencyConfiguration,
                    characterConfig.chattingAbilityGainEfficiencyInitialValue,
                    VRaisingEventKey.OnChattingAbilityGainEfficiencyChanged,
                    characterConfig.chattingAbilityGainEfficiencyMaxValue == -1 ? int.MaxValue : characterConfig.chattingAbilityGainEfficiencyMaxValue,
                    characterConfig.chattingAbilityGainEfficiencyMinValue));
            
            AttributeManager.AddAttribute("CAFollowerCount", 
                new VFollowerCountAttribute(characterConfig.followerCountConfiguration,
                    characterConfig.followerCountInitialValue, 
                    VRaisingEventKey.OnFollowerCountChanged, 
                    characterConfig.followerCountMaxValue == -1 ? int.MaxValue : characterConfig.followerCountMaxValue,
                    characterConfig.followerCountMinValue));
            
            AttributeManager.AddAttribute("CAMembershipCount",
                new VCharacterAttribute(characterConfig.membershipCountConfiguration,
                    characterConfig.membershipCountInitialValue, 
                    VRaisingEventKey.OnMemberCountChanged, 
                    characterConfig.membershipCountMaxValue == -1 ? int.MaxValue : characterConfig.membershipCountMaxValue,
                    characterConfig.membershipCountMinValue));
            
            AttributeManager.AddAttribute("CAFollowerToViewerRatio",
                new VConversionRatioAttribute(characterConfig.followerToViewerRatioConfiguration,
                    characterConfig.followerToViewerRatioInitialValue, 
                    VRaisingEventKey.OnFollowerToViewerRatioChanged, 
                    characterConfig.followerToViewerRatioMaxValue == -1 ? int.MaxValue : characterConfig.followerToViewerRatioMaxValue,
                    characterConfig.followerToViewerRatioMinValue));
            
            AttributeManager.AddAttribute("CAMoney",
                new VMoneyAttribute(characterConfig.moneyConfiguration,
                    characterConfig.moneyInitialValue, 
                    VRaisingEventKey.OnMoneyChanged, 
                    characterConfig.moneyMaxValue == -1 ? int.MaxValue : characterConfig.moneyMaxValue,
                    characterConfig.moneyMinValue));
        }
        
        
    }
}













