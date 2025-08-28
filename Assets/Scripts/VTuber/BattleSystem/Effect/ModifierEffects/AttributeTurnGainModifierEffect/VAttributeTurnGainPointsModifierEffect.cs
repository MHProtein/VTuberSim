using Spire.Xls;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect
{
    public class VAttributeTurnGainPointsModifierEffect : VEffect
    {
        public string attributeName;
        public int turnCount;
        public VUpgradableValue<int> deltaPoints;
        
        public VAttributeTurnGainPointsModifierEffect(VAttributeTurnGainPointsModifierEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            attributeName = configuration.attributeName;
            turnCount = configuration.turnCount;
            deltaPoints = new VUpgradableValue<int>(int.Parse(parameter), int.Parse(upgradedParameter));
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            base.ApplyEffect(battle, layer, isFromCard, shouldApplyTwice);
            
            if(battle.BattleAttributeManager.TryGetAttribute(attributeName, out var attribute))
            {
                float pointsValue = deltaPoints.Value;
                if(MultiplyByLayer > 0.0f)
                    pointsValue *= layer * MultiplyByLayer;
                
                attribute.GainRateModifier.AddModifier(pointsValue, turnCount);
            }
        }

        public override string GetValue()
        {
            return deltaPoints.Value.ToString();
        }
    }

    public class VAttributeTurnGainPointsModifierEffectConfiguration : VEffectConfiguration
    {
        public string attributeName;
        public int turnCount;
        public VAttributeTurnGainPointsModifierEffectConfiguration(CellRange row) : base(row)
        {
            var parameters = row.Columns[VEffectHeaderIndex.Parameter].Value.Split(',');
            attributeName = parameters[0].Trim();
            turnCount = int.Parse(parameters[1].Trim());
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            return new VAttributeTurnGainPointsModifierEffect(this, parameter, upgradedParameter);
        }
    }
}