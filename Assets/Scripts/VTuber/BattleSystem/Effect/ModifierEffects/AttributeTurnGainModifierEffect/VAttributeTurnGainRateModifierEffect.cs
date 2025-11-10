using System;
using Spire.Xls;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.Effect
{
    public class VAttributeTurnGainRateModifierEffect : VEffect
    {
        public string attributeName;
        public VUpgradableValue<float> deltaRate;
        public int turnCount;

        public VAttributeTurnGainRateModifierEffect(VAttributeTurnGainRateModifierEffectConfiguration configuration,
            string parameter, string upgradedParameter) : base(configuration)
        {
            attributeName = configuration.attributeName;
            turnCount = configuration.turnCount;
            try
            {
                deltaRate = new VUpgradableValue<float>(float.Parse(parameter), float.Parse(upgradedParameter));
            }
            catch (Exception e)
            {
                VDebug.LogError("deltaRate in VAttributeTurnGainRateModifierEffect, id :" + configuration.id);
                throw;
            }
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false,
            bool shouldApplyTwice = false)
        {
            base.ApplyEffect(battle, layer, isFromCard, shouldApplyTwice);

            if (battle.BattleAttributeManager.TryGetAttribute(attributeName, out var attribute))
            {
                var rateValue = deltaRate.Value;
                if (MultiplyByLayer > 0.0f)
                    rateValue *= layer * MultiplyByLayer;

                attribute.GainRateModifier.AddModifier(rateValue, turnCount);
            }
        }

        public override string GetValue()
        {
            return VMathUtils.FloatToInt(deltaRate.Value * 100) + "%";
        }
    }

    public class VAttributeTurnGainRateModifierEffectConfiguration : VEffectConfiguration
    {
        public string attributeName;
        public int turnCount;

        public VAttributeTurnGainRateModifierEffectConfiguration(CellRange row) : base(row)
        {
            var parameters = row.Columns[VEffectHeaderIndex.Parameter].Value.Split(',');
            attributeName = parameters[0].Trim();
            turnCount = int.Parse(parameters[1].Trim());
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VAttributeTurnGainRateModifierEffect(this, parameter, upgradedParameter);
        }
    }
}