using System;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect
{
    public class VStaminaCostModiferEffect : VEffect
    {
        private VStaminaCostModifierType _modifierType;
        
        private VUpgradableValue<float> _deltaRate;

        private VUpgradableValue<int> _deltaPoints;
        
        private float _multiplyByLayer;
        
        
        public VStaminaCostModiferEffect(VStaminaCostModiferEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _modifierType = configuration.modifierType;
            
            _deltaRate = new VUpgradableValue<float>(Convert.ToSingle(parameter), Convert.ToSingle(upgradedParameter));

            _deltaPoints = new VUpgradableValue<int>(Convert.ToInt32(parameter), Convert.ToInt32(upgradedParameter));
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            if(battle.BattleAttributeManager.TryGetAttribute("BAStamina", out var atrribute))
            {
                VBattleStaminaAttribute staminaAttribute = atrribute as VBattleStaminaAttribute;

                switch (_modifierType)
                {
                    case VStaminaCostModifierType.Rate:
                        float rateValue = _deltaRate.Value;
                        if(_multiplyByLayer > 0.0f)
                            rateValue *= layer * _multiplyByLayer;
                        staminaAttribute.ChangeConsumeRate(rateValue);
                        break;
                    case VStaminaCostModifierType.Points:             
                        float pointValue = _deltaPoints.Value;
                        if(_multiplyByLayer > 0.0f)
                            pointValue *= layer * _multiplyByLayer;
                        staminaAttribute.ChangeConsumeReducedPoints((int)pointValue);
                        break;
                }
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();
            _deltaRate.Upgrade();
            _deltaPoints.Upgrade();
        }
        
        public override void Downgrade()
        {
            base.Downgrade();
            _deltaRate.Downgrade();
            _deltaPoints.Downgrade();
        }
        
    }
}