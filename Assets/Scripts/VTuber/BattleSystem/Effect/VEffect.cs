using System.Collections.Generic;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect
{
    public abstract class VEffect
    {
        private VEffectConfiguration _configuration;

        public VEffect(VEffectConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public abstract void ApplyEffect(VBattle battle);

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