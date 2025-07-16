using System.Collections.Generic;
using VTuber.Character.Attribute;
using VTuber.Core.Foundation;

namespace VTuber.Character
{
    public class VCharacterConfiguration
    {
        public VCharacterAttributeConfiguration staminaConfiguration;
        public VCharacterAttributeConfiguration pressureConfiguration;
        
        public VCharacterAttributeConfiguration singingAbilityConfiguration;
        public VCharacterAttributeConfiguration gamingAbilityConfiguration;
        public VCharacterAttributeConfiguration chattingAbilityConfiguration;
        
        public VCharacterAttributeConfiguration singingAbilityConversionRatioConfiguration;
        public VCharacterAttributeConfiguration gamingAbilityConversionRatioConfiguration;
        public VCharacterAttributeConfiguration chattingAbilityConversionRatioConfiguration;
        
        public VCharacterAttributeConfiguration singingAbilityGainEfficiencyConfiguration;
        public VCharacterAttributeConfiguration gamingAbilityGainEfficiencyConfiguration;
        public VCharacterAttributeConfiguration chattingAbilityGainEfficiencyConfiguration;
        
        public VCharacterAttributeConfiguration followerCountConfiguration;
        public VCharacterAttributeConfiguration membershipCountConfiguration;
        
        public VCharacterAttributeConfiguration followerToViewerRatioConfiguration;
        
        public VCharacterAttributeConfiguration moneyConfiguration;
    }
}