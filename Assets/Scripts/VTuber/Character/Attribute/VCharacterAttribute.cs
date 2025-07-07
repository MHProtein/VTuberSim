using System.Collections.Generic;
using VTuber.BattleSystem.Core.Buff.Actions;
using VTuber.Character.AttributeConfigurations;

namespace VTuber.Character.Attribute
{
    public abstract class VCharacterAttribute
    {
        public bool IsConvertToBattleAttribute => _configuration.isConvertToBattleAttribute;
        private VCharacterAttributeConfiguration _configuration;
        
        public VCharacterAttribute(VCharacterAttributeConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public string GetAttributeName()
        {
            return _configuration.attributeName;   
        }

        public abstract object ConvertToBattleAttribute();

    }
    
    public class VCharacterIntegerAttribute : VCharacterAttribute
    {
        public int Value { get; private set; }

        public VCharacterIntegerAttribute(int value, VIntegerAttributeConfiguration configuration) : base(configuration)
        {
            Value = value;
        }

        public override object ConvertToBattleAttribute()
        {
            return Value;
        }
    }
    
    public class VCharacterFloatAttribute : VCharacterAttribute
    {
        public float Value { get; private set; }

        public VCharacterFloatAttribute(float value, VFloatAttributeConfiguration configuration) : base(configuration)
        {
            Value = value;
        }

        public override object ConvertToBattleAttribute()
        {
            return Value;
        }
    }
}