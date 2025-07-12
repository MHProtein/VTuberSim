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
        
        public VRootEventKey WhenToApply => _configuration.whenToApply;
        
        public int ConfigId => _configuration.buffId;
        
        public bool IsPermanent => _configuration.IsBuffPermanent();
        
        
        public VBuff(VBuffConfiguration configuration)
        {
            _configuration = configuration;
            _effects = new List<VEffect>();
            
            foreach (var effect in _configuration.effects)
            {
                _effects.Add(effect.CreateEffect());
            }
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>if true then remove</returns>


 

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