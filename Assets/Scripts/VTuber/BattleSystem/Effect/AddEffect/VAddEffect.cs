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

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            if (battle.BattleAttributeManager.TryGetAttribute(_configuration.attributeName, out var attribute))
            {
                int value = _configuration.addValue;
                if (_configuration.multiplyByLayer)
                    value *= layer;
                attribute.AddTo(value, isFromCard, shouldApplyTwice);
                VDebug.Log($"Effect{_configuration.effectName} added {_configuration.addValue} to {_configuration.attributeName}. New value: {attribute.Value}");
            }
        }
    }
}