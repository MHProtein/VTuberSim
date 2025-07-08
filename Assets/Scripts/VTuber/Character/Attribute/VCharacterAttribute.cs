using System.Collections.Generic;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;
using VTuber.Character.AttributeConfigurations;

namespace VTuber.Character.Attribute
{
    public class VCharacterAttribute
    {
        public bool IsConvertToBattleAttribute => _configuration.isConvertToBattleAttribute;
        private VCharacterAttributeConfiguration _configuration;
        
        public int Value { get; private set; }
        
        public VCharacterAttribute(VCharacterAttributeConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public string GetAttributeName()
        {
            return _configuration.attributeName;   
        }
        
        public string GetBattleAttributeName()
        {
            return IsConvertToBattleAttribute ? _configuration.attributeName : "";
        }

        public virtual VBattleAttribute ConvertToBattleAttribute()
        {
            if (!IsConvertToBattleAttribute)
            {
                return null;
            }
            
            return new VBattleAttribute(Value, _configuration.isBattleAttributePercentage);
        }
    }
}