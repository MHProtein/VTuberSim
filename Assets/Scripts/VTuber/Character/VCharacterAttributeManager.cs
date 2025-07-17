using System.Collections.Generic;
using VTuber.Character.Attribute;

namespace VTuber.Character
{
    public class VCharacterAttributeManager
    {
        public Dictionary<string, VCharacterAttribute> Attributes { get; set; }
        
        public VCharacterAttributeManager()
        {
            Attributes = new Dictionary<string, VCharacterAttribute>();
        }

        public void AddAttribute(string name, VCharacterAttribute attribute)
        {
            Attributes.TryAdd(name, attribute);
        }

        public bool TryGetAttributeValue(string name, out int value, out bool isPercentage)
        {
            if(Attributes.TryGetValue(name, out var attribute))
            {
                value = attribute.Value;
                isPercentage = attribute.IsPercentage;
                return true;
            }

            value = 0;
            isPercentage = false;
            return false;
        }
        
    }
}