using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.Effect
{
    public class VEffect
    {
        protected VEffectConfiguration _configuration;
        public List<VEffectCondition> conditions;
        public VBattleEventKey whenToApply;
        public bool upgradable = false;
        protected bool _isUpgraded;
        public float MultiplyByLayer => _configuration.multiplyByLayer;

        public int applyLatency;
        
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
            if (applyLatency > 0)
                return false;
            if (conditions == null || conditions.Count == 0)
                return true;

            foreach (var condition in conditions)
            {
                if (!condition.IsTrue(battle, message))
                {
                    return false;
                }
            }
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

        public virtual void OnBuffLayerChange(int layer)
        {
            
        }
        
        public virtual void OnBuffRemove()
        {
            
        }

        public void OnTurnEnd()
        {
            applyLatency--;
        }
        
    }
}