using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.EventCenter;
using VTuber.Core.RaisingEffect;

namespace VTuber.Relic
{
    public class VBattleRelic : VRelic
    {
        private uint _battleID;
        private uint _id;
        private VBattleRelicManager _manager;
        public VEffectCondition condition;
        public VBattleEventKey whenToApply;
        public List<VEffect> Effects { get; }
        public uint BattleID => _battleID;

        public VBattleRelic(VBattleRelicConfiguration config) : base(config)
        {
            Effects = new List<VEffect>();
            Effects.AddRange(config.effectItems.Select(item => item.CreateEffect()));
            condition = config.condition;
            whenToApply = config.whenToApply;
        }

        public VBattleRelic Copy()
        {
            return new VBattleRelic(configuration as VBattleRelicConfiguration);
        }
        
        public override void OnRelicAddedInRaising()
        {
            base.OnRelicAddedInRaising();
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRelicAdded,
                new Dictionary<string, object>
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
                new Dictionary<string, object>
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
                new Dictionary<string, object>
                {
                    { "Id", _battleID },
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
                new Dictionary<string, object>
                {
                    { "Id", _battleID },
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
                foreach (var effect in Effects) effect.ApplyEffect(_manager.Battle);

                if (!IsPermanent)
                {
                    layer--;
                    VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRelicValueChanged,
                        new Dictionary<string, object>
                        {
                            { "Id", _battleID },
                            { "IsPermanent", IsPermanent },
                            { "Value", Layer },
                            { "RelicName", GetRelicName() }
                        });

                    if (layer <= 0) _manager.Remove(this);
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
        private VRaisingRelicManager _manager;
        public VRaisingRelicCondition relicCondition;

        public VRaisingEventKey whenToApply;

        public VRaisingRelic(VRaisingRelicConfiguration config) : base(config)
        {
            Effects = new List<VRaisingEffect>();
            Effects.AddRange(config.effectItems.Select(item => item.CreateRaisingEffect()));
            relicCondition = config.condition;
            whenToApply = config.whenToApply;
        }

        public List<VRaisingEffect> Effects { get; }

        public override void OnRelicAddedInRaising()
        {
            base.OnRelicAddedInRaising();
            VRaisingRootEventCenter.Instance.RegisterListener(whenToApply, OnEventRaised);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRelicAdded,
                new Dictionary<string, object>
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
                new Dictionary<string, object>
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
                foreach (var effect in Effects)
                    effect.ApplyEffect(_manager.Character, messagedict, VInstigatorType.Relic, Icon);
                if (!IsPermanent)
                {
                    layer--;

                    VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRelicValueChanged,
                        new Dictionary<string, object>
                        {
                            { "Id", _id },
                            { "IsPermanent", IsPermanent },
                            { "Value", Layer },
                            { "RelicName", GetRelicName() },
                            { "IsStreamRelic", false }
                        });

                    if (layer <= 0) _manager.Remove(this);
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
        protected readonly VRelicConfiguration configuration;
        protected uint _id;
        protected int layer;

        public VRelic(VRelicConfiguration config)
        {
            configuration = config;
            layer = config.layer;
            if (layer == -1)
                IsPermanent = true;
        }

        public uint Id => _id;
        public string Description => configuration.description;
        public uint ConfigId => configuration.id;

        public Sprite Icon => configuration.icon;
        public int Layer => layer;

        public bool IsPermanent { get; }

        public string GetRelicName()
        {
            return configuration.relicName;
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