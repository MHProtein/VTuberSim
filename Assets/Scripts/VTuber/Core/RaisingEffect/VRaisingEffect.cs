using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingEffect
    {
        protected VRaisingEffectConfiguration _configuration;
        
        public uint Id => _configuration.id;
        public string Name => _configuration.effectName;
        public string Description => _configuration.description;
        
        public VRaisingEffect(VRaisingEffectConfiguration configuration)
        {
            _configuration = configuration;
        }

            
        public virtual void ApplyEffect(VCharacter character)
        {
            
        }

        public virtual void Upgrade()
        {
            
        }

        public virtual void DownGrade()
        {
            
        }
        
    }
}