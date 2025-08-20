using System;
using System.Collections.Generic;
using System.Linq;
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
        private uint _id;
        
        public VBattleRelic(VBattleRelicConfiguration config) : base(config)
        {
            _effects = new List<VEffect>();
            _effects.AddRange(config.effectItems.Select(item => item.CreateEffect()));
            condition = config.condition;
            whenToApply = config.whenToApply;
        }

        public override void OnRelicAdded()
        {
            base.OnRelicAdded();
            VBattleRootEventCenter.Instance.RegisterListener(whenToApply, OnEventRaised);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRelicAdded,
                new Dictionary<string, object>()
                {
                    { "Id", _id },
                    { "IsPermanent", IsPermanent },
                    { "Value", Layer },
                    { "RelicName", GetRelicName() }
                });
        }

        public override void OnRelicRemoved()
        {
            base.OnRelicRemoved();
            VBattleRootEventCenter.Instance.RemoveListener(whenToApply, OnEventRaised);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRelicRemoved,
                new Dictionary<string, object>()
                {
                    { "Id", _id },
                    { "IsPermanent", IsPermanent },
                    { "Value", Layer },
                    { "RelicName", GetRelicName() }
                });
        }

        public bool CanApply(VBattle battle, Dictionary<string, object> message)
        {
            if (condition is null)
                return true;
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

                if (!IsPermanent)
                {
                    layer--;
                    VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRelicValueChanged,
                        new Dictionary<string, object>()
                        {
                            { "Id", _id },
                            { "IsPermanent", IsPermanent },
                            { "Value", Layer },
                            { "RelicName", GetRelicName() }
                        });
                
                    if (layer <= 0)
                    {
                        _manager.Remove(this);
                    }
                }
            }
        }

        public void Initialize(uint id, VBattleRelicManager manager)
        {
            _id = id;
            _manager = manager;
        }
    }

    public class VRaisingRelic : VRelic
    {
        private uint _id;
        public List<VRaisingEffect> Effects => _effects;
        private List<VRaisingEffect> _effects;
        private VRaisingRelicManager _manager;
        
        public VRaisingEventKey whenToApply;
        public VRaisingRelicCondition relicCondition;
        public VRaisingRelic(VRaisingRelicConfiguration config) : base(config)
        {
            _effects = new List<VRaisingEffect>();
            _effects.AddRange(config.effectItems.Select(item => item.CreateRaisingEffect()));
            relicCondition = config.condition;
            whenToApply = config.whenToApply;
        }
        
        public override void OnRelicAdded()
        {
            base.OnRelicAdded();
            VRaisingRootEventCenter.Instance.RegisterListener(whenToApply, OnEventRaised);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRelicAdded,
                new Dictionary<string, object>()
                {
                    { "Id", _id },
                    { "IsPermanent", IsPermanent },
                    { "Value", Layer },
                    { "RelicName", GetRelicName() }
                });
        }

        public override void OnRelicRemoved()
        {
            base.OnRelicRemoved();
            VRaisingRootEventCenter.Instance.RemoveListener(whenToApply, OnEventRaised);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRelicRemoved,
                new Dictionary<string, object>()
                {
                    { "Id", _id },
                    { "IsPermanent", IsPermanent },
                    { "Value", Layer },
                    { "RelicName", GetRelicName() }
                });
        }
        
        public bool CanApply(Dictionary<string, object> message)
        {
            if (relicCondition is null)
                return true;
            return relicCondition.IsTrue(_manager.Character, message);
        }
        
        private void OnEventRaised(Dictionary<string, object> messagedict)
        {
            if (CanApply(messagedict))
            {
                foreach (var effect in _effects)
                    effect.ApplyEffect(_manager.Character, messagedict);
                if (!IsPermanent)
                {
                    layer--;
                
                    VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRelicValueChanged,
                        new Dictionary<string, object>()
                        {
                            { "Id", _id },
                            { "IsPermanent", IsPermanent },
                            { "Value", Layer },
                            { "RelicName", GetRelicName() }
                        });
                    
                    if (layer <= 0)
                    {
                        _manager.Remove(this);
                    }
                }
            }
        }
        
        public void Initialize(uint id, VRaisingRelicManager manager)
        {
            _id = id;
            _manager = manager;
        }
    }
    
    public class VRelic
    {
        private VRelicConfiguration _configuration;
        public uint ConfigId => _configuration.id;
        public string GetRelicName() => _configuration.relicName;

        public int Layer => layer;
        protected int layer;

        public bool IsPermanent => _isPermanent;
        private bool _isPermanent = false;
        
        public VRelic(VRelicConfiguration config)
        {
            _configuration = config;
            layer = config.layer;
            if (layer == -1)
                _isPermanent = true;
        }

        public virtual void OnRelicAdded()
        {
            
        }

        public virtual void OnRelicRemoved()
        {
            
        }
    }
}
