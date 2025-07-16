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
        
    }
}