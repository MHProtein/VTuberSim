using System;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect
{
    public class VAddEffect : VEffect
    {
        private VUpgradableValue<int> _addValue;
        VAddEffectConfiguration _configuration;
        public VAddEffect(VAddEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _configuration = configuration;
            
            _addValue = new VUpgradableValue<int>(Convert.ToInt32(parameter), Convert.ToInt32(upgradedParameter));
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            if (battle.BattleAttributeManager.TryGetAttribute(_configuration.attributeName, out var attribute))
            {
                int value = _addValue.Value;
                if (MultiplyByLayer > 0.0f)
                    value *= (int)(layer * MultiplyByLayer);
                attribute.AddTo(value, isFromCard, shouldApplyTwice);
                VDebug.Log($"Effect{_configuration.effectName} added {_addValue.Value} to {_configuration.attributeName}. New value: {attribute.Value}");
            }
        }
    }
}