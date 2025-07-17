using System;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect
{
    public class VAttributeGainRateModifierEffect : VEffect
    {
        private readonly string _attributeName;
        private readonly VUpgradableValue<float> _deltaRate;

        private Action<uint> _onBuffRemove;
        private Action<uint, float> _onBuffLayerChangeRate;
        private Action<uint, int> _onBuffLayerChangePoints;
        private uint modifierID;
        private bool applied = false;
        
        public VAttributeGainRateModifierEffect(VAttributeGainRateModifierEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _attributeName = configuration.attributeName;
            
            _deltaRate = new VUpgradableValue<float>(Convert.ToSingle(parameter), Convert.ToSingle(upgradedParameter));
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            if (applied)
                return;
            applied = true;
            if(battle.BattleAttributeManager.TryGetAttribute(_attributeName, out var atrribute))
            {
                float rateValue = _deltaRate.Value;
                if(MultiplyByLayer > 0.0f)
                    rateValue *= layer * MultiplyByLayer;
                
                modifierID = atrribute.GainRateModifier.AddModifier(rateValue);
                _onBuffRemove = atrribute.GainRateModifier.RemoveModifier;
                _onBuffLayerChangeRate = atrribute.GainRateModifier.ChangeModifier;
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();
            _deltaRate.Upgrade();
        }
        
        public override void Downgrade()
        {
            base.Downgrade();
            _deltaRate.Downgrade();
        }

        public override void OnBuffLayerChange(int layer)
        {
            if (MultiplyByLayer < 0.0f)
                return;
    
            float rateValue = _deltaRate.Value;
            rateValue *= layer * MultiplyByLayer;
            _onBuffLayerChangeRate(modifierID, rateValue);
        }

        public override void OnBuffRemove()
        {
            _onBuffRemove(modifierID);
        }
    }
}