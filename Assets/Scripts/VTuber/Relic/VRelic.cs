using System.Collections.Generic;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.EventCenter;
using VTuber.Core.RaisingEffect;

namespace VTuber.Relic
{
    public class VBattleRelic : VRelic
    {      
        public List<VEffect> Effects => _effects;
        private List<VEffect> _effects;
        private VBattleRelicManager _manager;
        public VBattleEventKey whenToApply;
        public VEffectCondition condition;
        
        public VBattleRelic(VRelicConfiguration config) : base(config)
        {
            
        }

        public override void OnRelicAdded()
        {
            base.OnRelicAdded();
            VBattleRootEventCenter.Instance.RegisterListener(whenToApply, OnEventRaised);
        }

        public override void OnRelicRemoved()
        {
            base.OnRelicRemoved();
            VBattleRootEventCenter.Instance.RemoveListener(whenToApply, OnEventRaised);
        }

        public bool CanApply(VBattle battle, Dictionary<string, object> message)
        {
            return condition.IsTrue(battle, message);
        }

        private void OnEventRaised(Dictionary<string, object> messagedict)
        {
            if (CanApply(_manager.Battle, messagedict))
            {
                foreach (var effect in _effects)
                {
                    effect.ApplyEffect(_manager.Battle);
                }
                layer--;
                if (layer <= 0)
                {
                    _manager.Remove(this);
                }
            }
        }
    }

    public class VRaisingRelic : VRelic
    {
        public List<VRaisingEffect> Effects => _effects;
        private List<VRaisingEffect> _effects;
        private VRaisingRelicManager _manager;
        
        public VRaisingEventKey whenToApply;
        public VRaisingRelicCondition relicCondition;
        public VRaisingRelic(VRelicConfiguration config) : base(config)
        {
        }
        
        public override void OnRelicAdded()
        {
            base.OnRelicAdded();
            VRaisingRootEventCenter.Instance.RegisterListener(whenToApply, OnEventRaised);
        }

        public override void OnRelicRemoved()
        {
            base.OnRelicRemoved();
            VRaisingRootEventCenter.Instance.RemoveListener(whenToApply, OnEventRaised);
        }
        
        public bool CanApply(Dictionary<string, object> message)
        {
            return relicCondition.IsTrue(_manager.Character, message);
        }
        
        private void OnEventRaised(Dictionary<string, object> messagedict)
        {
            if (CanApply(messagedict))
            {
                foreach (var effect in _effects)
                    effect.ApplyEffect(_manager.Character);
                
                layer--;
                if (layer <= 0)
                {
                    _manager.Remove(this);
                }
            }
        }
    }
    
    public class VRelic
    {
        private VRelicConfiguration _configuration;
        public uint ConfigId => _configuration.id;
        public string GetRelicName() => _configuration.relicName;

        public int Layer => layer;
        protected int layer;
        
        public VRelic(VRelicConfiguration config)
        {
            _configuration = config;
            layer = config.layer;
        }

        public virtual void OnRelicAdded()
        {
            
        }

        public virtual void OnRelicRemoved()
        {
            
        }
    }
}
