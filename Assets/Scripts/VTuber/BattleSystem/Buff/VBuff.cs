using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Buff
{
    public class VBuff
    {
        public VRootEventKey WhenToApply => _configuration.whenToApply;
        
        private VBuffConfiguration _configuration;

        private VBattle _battle;

        private List<VEffect> _effects;
        
        public int Duration { get; private set; }
        public int Layer { get; private set; }
        
        public VBuff(VBuffConfiguration configuration)
        {
            _configuration = configuration;
            Duration = _configuration.duration;
            _effects = new List<VEffect>();
            
            foreach (var effect in _configuration.effects)
            {
                _effects.Add(effect.CreateEffect());
            }
            
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
            VDebug.Log($"{_configuration.buffName} duration decremented to {Duration}");
            return false;
        }

        public void ApplyBuff(Dictionary<string, object> message)
        {
            if (_configuration.effects == null || _configuration.effects.Count == 0)
                return;
            
            foreach (var effect in _effects)
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
            if (_configuration.IsBuffPermanent())
            {
                Layer += buff.Layer;
                VDebug.Log($"{_configuration.buffName} stacked to {Layer} layers");
            }
            else
            {
                Duration += buff.Duration;
                VDebug.Log($"{_configuration.buffName} stacked to {Duration} turns");
            }
            
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