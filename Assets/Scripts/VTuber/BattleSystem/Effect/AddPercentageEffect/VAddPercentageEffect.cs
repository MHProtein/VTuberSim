using System;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect.AddPercentageEffect
{
    public class VAddPercentageEffect : VEffect, IVValuePreview
    {
        private string _attributeName;
        public string AttributeName => _attributeName;
        private VUpgradableValue<float> _percentage;
        public VAddPercentageEffect(VAddPercentageEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _attributeName = configuration.attributeName;
            _percentage = new VUpgradableValue<float>(Convert.ToSingle(parameter), Convert.ToSingle(upgradedParameter));
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            base.ApplyEffect(battle, layer, isFromCard, shouldApplyTwice);
            if (battle.BattleAttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                int delta = (int)(_percentage.Value * attribute.Value);
                if (MultiplyByLayer > 0.0f)
                    delta *= (int)(layer * MultiplyByLayer);
                attribute.AddTo(delta, isFromCard, shouldApplyTwice);
                VDebug.Log($"Effect{_configuration.effectName} added {_percentage.Value} to {_attributeName}. New value: {attribute.Value}");
            }   
        }

        public int GetValue(VBattle battle)
        {
            if (battle.BattleAttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                return (int)(_percentage.Value * attribute.Value);
            }
            return 0;
        }
        
        public override string GetValue()
        {
            return (int)(_percentage.Value * 100) + "%";
        }
    }
}