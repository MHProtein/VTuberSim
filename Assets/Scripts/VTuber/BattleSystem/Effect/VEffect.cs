using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect
{
    public class VEffect
    {
        protected VEffectConfiguration _configuration;
        public uint Id => _configuration.id;
        public string Name => _configuration.effectName;
        public string Description => _configuration.description;
        //public string Icon => _configuration.icon;
        //public string UpgradeIcon => _configuration.upgradeIcon;
        
        public List<VEffectCondition> conditions;
        public VBattleEventKey whenToApply;
        public bool upgradable = false;
        protected bool _isUpgraded;
        protected VBattle _battle;
        protected int _layer = 0;
        public float MultiplyByLayer => _configuration.multiplyByLayer;
        
        public VEffect(VEffectConfiguration configuration)
        {
            _configuration = configuration;
            conditions = configuration.conditions;
            whenToApply = configuration.whenToApply;
            upgradable = configuration.upgradable;
        }

        public virtual void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            
        }

        public bool CanApply(VBattle battle, Dictionary<string, object> message)
        {
            if (conditions == null || conditions.Count == 0)
            {
                VDebug.Log("Effect " + Name + " can be applied without conditions.");
                return true;
            }
            
            foreach (var condition in conditions)
            {
                if (!condition.IsTrue(battle, message))
                {
                    VDebug.Log("Effect " + Name + " cannot be applied due to condition: " + condition.id);
                    return false;
                }
            }
            VDebug.Log("Effect " + Name + " can be applied.");
            return true;
        }
        
        public virtual void Upgrade()
        {
            if(!upgradable)
                return;
            _isUpgraded = true;
        }
        
        public virtual void Downgrade()
        {            
            if(!upgradable)
                return;
            _isUpgraded = false;
        }

        public void TryApply(Dictionary<string, object> dict)
        {
            if (CanApply(_battle, dict))
                ApplyEffect(_battle, _layer);
        }
        
        public virtual void OnBuffAdded(VBattle battle, int layer)
        {
            _battle = battle;
            _layer = layer;
            VBattleRootEventCenter.Instance.RegisterListener(whenToApply, TryApply);
        }
        
        public virtual void OnBuffLayerChange(int layer)
        {
            _layer = layer;
        }
        
        public virtual void OnBuffRemove()
        {
            VBattleRootEventCenter.Instance.RemoveListener(whenToApply, TryApply);
        }
    }
}