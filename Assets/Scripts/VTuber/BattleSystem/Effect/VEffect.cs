using System.Collections.Generic;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect
{
    public class VEffect
    {
        protected VEffectConfiguration _configuration;

        public VEffect(VEffectConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual void ApplyEffect(VBattle battle)
        {
            
        }

        public bool AreConditionsMet(VBattle battle, Dictionary<string, object> message)
        {
            foreach (var condition in _configuration.conditions)
            {
                if (!condition.IsTrue(battle, message))
                {
                    return false;
                }
            }

            return true;
        }
        
    }
}