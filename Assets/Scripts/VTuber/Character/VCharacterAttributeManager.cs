using System.Collections.Generic;
using VTuber.Character.Attribute;

namespace VTuber.Character
{
    public class VCharacterAttributeManager
    {
        public Dictionary<string, VCharacterAttribute> Attributes { get; set; }
        
        public VCharacterAttributeManager(VCharacterAttributeManagerConfiguration configuration)
        {
            Attributes = new Dictionary<string, VCharacterAttribute>();

            foreach (var attribute in configuration.attributes)  
            {
                Attributes.Add(attribute.attributeName, attribute.GetAttribute());
            }
        }
    }
}