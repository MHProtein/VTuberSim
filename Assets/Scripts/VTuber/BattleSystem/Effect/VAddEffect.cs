using UnityEngine;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect
{
    public class VAddEffect : VEffect
    {
        VAddEffectConfiguration _configuration;
        public VAddEffect(VAddEffectConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public override void ApplyEffect(VBattle battle)
        {
            if (battle.BattleAttributeManager.TryGetAttribute(_configuration.attributeName, out var attribute))
            {
                attribute.AddTo(_configuration.addValue);
                Debug.Log(attribute.Value);
            }

        }
    }
}