using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

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
                VDebug.Log($"Effect{_configuration.effectName} added {_configuration.addValue} to {_configuration.attributeName}. New value: {attribute.Value}");
            }
        }
    }
}