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

        private VBattle _battle;

        private List<VEffect> _effects;
        
        public VRootEventKey WhenToApply => _configuration.whenToApply;
        
        public int Duration { get; private set; }
        
        public int Layer { get; private set; }
        
        public int Id { get; private set; }
        
        public bool IsPermanent => _configuration.IsBuffPermanent();
        
        public VBuff(VBuffConfiguration configuration)
        {
            _configuration = configuration;
            Duration = _configuration.duration;
            Layer = configuration.layer;
            _effects = new List<VEffect>();
            
            foreach (var effect in _configuration.effects)
            {
                _effects.Add(effect.CreateEffect());
            }
            
        }

        public void OnBuffAdded(VBattle battle, int id)
        {
            Id = id;
            _battle = battle;
            VBattleRootEventCenter.Instance.RegisterListener(WhenToApply, ApplyBuff);
        }
        
        public void OnBuffRemoved()
        {
            VBattleRootEventCenter.Instance.RemoveListener(WhenToApply, ApplyBuff);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>if true then remove</returns>
        public bool DecrementDuration()
        {
            if (IsPermanent)
                return false;
            
            Duration -= 1;
            if (Duration <= 0)
                return true;
            
            VDebug.Log($"{_configuration.buffName} duration decremented to {Duration}");
            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnBuffValueUpdated, new Dictionary<string, object>
            {
                { "Id", Id },
                {"Value", Duration}
            });
            return false;
        }

        public void ApplyBuff(Dictionary<string, object> message)
        {
            if (_configuration.effects == null || _configuration.effects.Count == 0)
                return;

            for (int i = 0; i < Layer; i++)
            {
                foreach (var effect in _effects)
                {
                    if(effect.AreConditionsMet(_battle, message))
                        effect.ApplyEffect(_battle);
                }
            }
        }

        public virtual bool IsStackable(VBuff buff)
        {
            return _configuration.stackable;
        }
        
        public virtual void Stack(VBuff buff)
        {
            int value = 0;
            if (_configuration.IsBuffPermanent())
            {
                Layer += buff.Layer;
                VDebug.Log($"{_configuration.buffName} stacked to {Layer} layers");
                value = Layer;
            }
            else
            {
                Duration += buff.Duration;
                VDebug.Log($"{_configuration.buffName} stacked to {Duration} turns");
                value = Duration;
            }
            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnBuffValueUpdated, new Dictionary<string, object>
            {
                { "Id", Id },
                {"Value", value}
            });
        }
        
        public string GetBuffName()
        {
            return _configuration.buffName;
        }

        public string GetAttributeToApplyName()
        {
            return _configuration.battleAttributeToApplyName;
        }

        public void AddLayerOrDuration(int value)
        {
            if (_configuration.IsBuffPermanent())
            {
                Layer += value;
                VDebug.Log($"{_configuration.buffName} layer increased to {Layer}");
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnBuffValueUpdated, new Dictionary<string, object>
                {
                    { "Id", Id },
                    {"Value", Layer}
                });
            }
            else
            {
                Duration += value;
                VDebug.Log($"{_configuration.buffName} duration increased to {Duration}");
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnBuffValueUpdated, new Dictionary<string, object>
                {
                    { "Id", Id },
                    {"Value", Duration}
                });
            } 
        }
    }
}