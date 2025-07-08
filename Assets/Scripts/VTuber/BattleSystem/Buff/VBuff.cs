using System.Collections.Generic;

namespace VTuber.BattleSystem.Buff
{
    public abstract class VBuff
    {
        private VBuffConfiguration _configuration;
        
        public VBuff(VBuffConfiguration configuration)
        {
            _configuration = configuration;
        }

        public abstract int ApplyBuff(int value);

        public virtual bool IsStackable(VBuff buff)
        {
            return _configuration.stackable;
        }
        
        public virtual void Stack(VBuff buff)
        {
            
        }
        
        public string GetBuffName()
        {
            return _configuration.buffName;
        }

        public string GetAttributeToApplyName()
        {
            return _configuration.battleAttributeToApplyName;
        }
        
    }
}