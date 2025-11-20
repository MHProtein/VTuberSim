using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.Effect.AddPercentageEffect
{
    public class VAddPercentageEffect : VEffect, IVValuePreview
    {
        private readonly VUpgradableValue<float> _percentage;

        public VAddPercentageEffect(VAddPercentageEffectConfiguration configuration, string parameter,
            string upgradedParameter) : base(configuration)
        {
            AttributeName = configuration.attributeName;
            _percentage = new VUpgradableValue<float>(Convert.ToSingle(parameter), Convert.ToSingle(upgradedParameter));
        }

        public string AttributeName { get; }

        public int GetValue(VBattle battle)
        {
            if (battle.BattleAttributeManager.TryGetAttribute(AttributeName, out var attribute))
                return VMathUtils.FloatToInt(_percentage.Value * attribute.Value);
            return 0;
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false,
            bool shouldApplyTwice = false)
        {
            base.ApplyEffect(battle, layer, isFromCard, shouldApplyTwice);
            if (battle.BattleAttributeManager.TryGetAttribute(AttributeName, out var attribute))
            {
                var delta = VMathUtils.FloatToInt(_percentage.Value * attribute.Value);
                if (MultiplyByLayer > 0.0f)
                    delta *= VMathUtils.FloatToInt(layer * MultiplyByLayer);

                if (delta == 0 && isFromCard)
                {
                    VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                        new Dictionary<string, object>());
                    return;
                }

                attribute.AddTo(delta, isFromCard, shouldApplyTwice);
                VDebug.Log(
                    $"Effect{_configuration.effectName} added {_percentage.Value} to {AttributeName}. New value: {attribute.Value}");
            }
        }

        public override string GetValue()
        {
            return VMathUtils.FloatToInt(_percentage.Value * 100, VMathUtils.RoundingType.Round) + "%";
        }
    }
}