using System;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect
{
    public class VAttributeGainPointsModifierEffect : VEffect
    {
        private string _attributeName;

        private readonly VUpgradableValue<int> _deltaPoints;

        private Action<uint> _onBuffRemove;
        private Action<uint, float> _onBuffLayerChangeRate;
        private Action<uint, int> _onBuffLayerChangePoints;
        private uint modifierID;
        private bool applied = false;
        
        public VAttributeGainPointsModifierEffect(VAttributeGainPointsModifierEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _attributeName = configuration.attributeName;
            _deltaPoints = new VUpgradableValue<int>(Convert.ToInt32(parameter), Convert.ToInt32(upgradedParameter));
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            if (applied)
                return;
            applied = true;
            if(battle.BattleAttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                float pointValue = _deltaPoints.Value;
                if(MultiplyByLayer > 0.0f)
                    pointValue *= layer * MultiplyByLayer;
                
                modifierID = attribute.GainPointsModifier.AddModifier((int)pointValue);
                _onBuffRemove = attribute.GainPointsModifier.RemoveModifier;
                _onBuffLayerChangePoints = attribute.GainPointsModifier.ChangeModifier;
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();
            _deltaPoints.Upgrade();
        }
        
        public override void Downgrade()
        {
            base.Downgrade();
            _deltaPoints.Downgrade();
        }

        public override void OnBuffLayerChange(int layer)
        {
            if (MultiplyByLayer < 0.0f)
                return;

            float pointValue = _deltaPoints.Value;
            pointValue *= layer * MultiplyByLayer;
            _onBuffLayerChangePoints(modifierID, (int)pointValue);
        }

        public override void OnBuffRemove()
        {
            _onBuffRemove(modifierID);
        }
    }
}