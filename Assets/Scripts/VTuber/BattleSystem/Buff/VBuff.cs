using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Buff
{
    public class VBuff
    {
        
        private VBuffConfiguration _configuration;
        
        public List<VEffect> Effects => _effects;
        private List<VEffect> _effects;
        
        public int ConfigId => _configuration.id;
        
        public bool IsPermanent => _configuration.IsBuffPermanent();
        
        
        public VBuff(VBuffConfiguration configuration, List<VEffect> effects)
        {
            _configuration = configuration;
            _effects = effects;
        }

        public virtual bool IsStackable()
        {
            return _configuration.stackable;
        }
        
        public string GetBuffName()
        {
            return _configuration.buffName;
        }
    }
}