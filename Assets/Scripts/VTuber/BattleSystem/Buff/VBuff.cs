using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.Buff
{
    public abstract class VBuff
    {
        public int Layer => _configuration.layer;
        
        public VRootEventKeys WhenToApply => _configuration.whenToApply;
        
        private VBuffConfiguration _configuration;

        private VBattle _battle;

        public int Duration { get; private set; }
        
        public VBuff(VBuffConfiguration configuration)
        {
            _configuration = configuration;
            Duration = _configuration.duration;
        }

        public void OnBuffAdded(VBattle battle)
        {
            _battle = battle;
            VRootEventCenter.Instance.RegisterListener(WhenToApply, ApplyBuff);
        }
        
        public void OnBuffRemoved()
        {
            VRootEventCenter.Instance.RemoveListener(WhenToApply, ApplyBuff);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>if true then remove</returns>
        public bool DecrementDuration()
        {
            if (_configuration.IsBuffPersistent())
            {
                Duration -= 1;
                if (Duration <= 0)
                    return true;
            }

            return false;
        }

        public void ApplyBuff(Dictionary<string, object> message)
        {
            if (_configuration.effects == null || _configuration.effects.Count == 0)
                return;
            
            foreach (var effect in _configuration.effects)
            {
                if(effect.AreConditionsMet(_battle, message))
                    effect.ApplyEffect(_battle);
            }
        }

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