using System.Collections.Generic;
using VTuber.Character.Attribute;

namespace VTuber.Character
{
    public class VCharacterAttributeManager
    {
        public List<VCharacterAttribute> Attributes { get; set; }
        
        public VCharacterAttributeManager(VCharacterAttributeManagerConfiguration configuration)
        {
            Attributes = new List<VCharacterAttribute>();

            foreach (var attribute in configuration.attributes)  
            {
                Attributes.Add(attribute.GetAttribute());
            }
        }
        
    }
}