using System.Collections.Generic;
using VTuber.Character.Attribute;

namespace VTuber.Character
{
    public class VCharacter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        
        public VCharacter(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}