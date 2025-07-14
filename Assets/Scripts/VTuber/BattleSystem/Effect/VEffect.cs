using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect.Conditions;

namespace VTuber.BattleSystem.Effect
{
    public class VEffect
    {
        protected VEffectConfiguration _configuration;
        public VEffectCondition condition;

        public VEffect(VEffectConfiguration configuration)
        {
            _configuration = configuration;
            condition = configuration.condition;
        }

        public virtual void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            
        }

        public bool CanApply(VBattle battle, Dictionary<string, object> message)
        {
            //return condition.IsTrue(battle, message);
            return true;
        }
        
    }
}