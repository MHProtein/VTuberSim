using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        private uint _battleID;
        
        public VBattleRelic(VBattleRelicConfiguration config) : base(config)
        {
            _effects = new List<VEffect>();
            _effects.AddRange(config.effectItems.Select(item => item.CreateEffect()));
            condition = config.condition;
            whenToApply = config.whenToApply;
        }
        
        public override void OnRelicAddedInRaising()
        {
            base.OnRelicAddedInRaising();
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRelicAdded,
                new Dictionary<string, object>()
                {
                    { "Id", _id },
                    { "IsPermanent", IsPermanent },
                    { "Relic", this },
                    { "Value", Layer },
                    { "RelicName", GetRelicName() },
                    { "IsStreamRelic", true }
                });
        }

        public override void OnRelicRemovedInRaising()
        {
            base.OnRelicRemovedInRaising();
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRelicRemoved,
                new Dictionary<string, object>()
                {
                    { "Id", _id },
                    { "IsPermanent", IsPermanent },
                    { "Relic", this },
                    { "Value", Layer },
                    { "RelicName", GetRelicName() },
                    { "IsStreamRelic", true }
                });
        }

        public void OnRelicAddedInBattle()
        {
            VBattleRootEventCenter.Instance.RegisterListener(whenToApply, OnEventRaised);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRelicAdded,
                new Dictionary<string, object>()
                {
                    { "Id", _id },
                    { "IsPermanent", IsPermanent },
                    { "Relic", this },
                    { "Value", Layer },
                    { "RelicName", GetRelicName() }
                });
        }

        public void OnRelicRemovedInBattle()
        {
            VBattleRootEventCenter.Instance.RemoveListener(whenToApply, OnEventRaised);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRelicRemoved,
                new Dictionary<string, object>()
                {
                    { "Id", _id },
                    { "IsPermanent", IsPermanent },
                    { "Relic", this },
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

        public void Initialize(uint id)
        {
            _id = id;
        }
        
        public void Initialize(uint id, VBattleRelicManager manager)
        {
            _battleID = id;
            _manager = manager;
        }
    }

    public class VRaisingRelic : VRelic
    {
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
        
        public override void OnRelicAddedInRaising()
        {
            base.OnRelicAddedInRaising();
            VRaisingRootEventCenter.Instance.RegisterListener(whenToApply, OnEventRaised);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRelicAdded,
                new Dictionary<string, object>()
                {
                    { "Id", _id },
                    { "IsPermanent", IsPermanent },
                    { "Relic", this },
                    { "Value", Layer },
                    { "RelicName", GetRelicName() },
                    { "IsStreamRelic", false }
                });
        }

        public override void OnRelicRemovedInRaising()
        {
            base.OnRelicRemovedInRaising();
            VRaisingRootEventCenter.Instance.RemoveListener(whenToApply, OnEventRaised);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRelicRemoved,
                new Dictionary<string, object>()
                {
                    { "Id", _id },
                    { "IsPermanent", IsPermanent },
                    { "Relic", this },
                    { "Value", Layer },
                    { "RelicName", GetRelicName() },
                    { "IsStreamRelic", false }
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
                            { "RelicName", GetRelicName() },
                            { "IsStreamRelic", false }
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
        public uint Id => _id;
        protected uint _id;
        public string Description => _configuration.description;
        private VRelicConfiguration _configuration;
        public uint ConfigId => _configuration.id;
        public string GetRelicName() => _configuration.relicName;
        public Sprite Icon => _configuration.icon;
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
        
        public void LoadLayer(int newLayer)
        {
            layer = newLayer;
        }

        public virtual void OnRelicAddedInRaising()
        {
            
        }

        public virtual void OnRelicRemovedInRaising()
        {
            
        }
    }
}
