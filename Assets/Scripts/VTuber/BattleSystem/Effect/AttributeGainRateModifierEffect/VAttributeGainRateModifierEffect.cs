using System;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

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

        public override void OnBuffAdded(VBattle battle, int layer)
        {            
            if(battle.BattleAttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                float rateValue = _deltaRate.Value;
                if(MultiplyByLayer > 0.0f)
                    rateValue *= layer * MultiplyByLayer;
                
                modifierID = attribute.GainRateModifier.AddModifier(rateValue);
                _onBuffRemove = attribute.GainRateModifier.RemoveModifier;
                _onBuffLayerChangeRate = attribute.GainRateModifier.ChangeModifier;
                VDebug.Log("Effect " + _configuration.effectName + " added " + _deltaRate.Value + " gain rate modifier with ID: " + modifierID);
            }
        }

        public override void OnBuffLayerChange(int layer)
        {
            if (MultiplyByLayer < 0.0f)
                return;
    
            float rateValue = _deltaRate.Value;
            rateValue *= layer * MultiplyByLayer;
            _onBuffLayerChangeRate(modifierID, rateValue);
            VDebug.Log("Effect " + _configuration.effectName + " changed gain rate to " + rateValue + " for layer " + layer);
        }

        public override void OnBuffRemove()
        {
            if (_onBuffRemove is null)
            {
                VDebug.LogError("OnBuffRemove is null for modifierID: " + modifierID + ", attribute: " + _attributeName + "检查属性名");
                return;
            }
            _onBuffRemove(modifierID);
        }
    }
}