using System;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect
{
    public class VStaminaCostModiferEffect : VEffect
    {
        private VStaminaCostModifierType _modifierType;
        
        private float _deltaRate;

        private int _deltaPoints;
        
        private bool _multiplyByLayer;
        
        public VStaminaCostModiferEffect(VStaminaCostModiferEffectConfiguration configuration) : base(configuration)
        {
            _modifierType = configuration.modifierType;
            _deltaRate = configuration.deltaRate;
            _deltaPoints = configuration.deltaPoints;
            _multiplyByLayer = configuration.multiplyByLayer;
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            if(battle.BattleAttributeManager.TryGetAttribute("BAStamina", out var atrribute))
            {
                VBattleStaminaAttribute staminaAttribute = atrribute as VBattleStaminaAttribute;

                switch (_modifierType)
                {
                    case VStaminaCostModifierType.Rate:
                        staminaAttribute.ChangeConsumeRate(_multiplyByLayer ? _deltaRate * layer : _deltaRate);
                        break;
                    case VStaminaCostModifierType.Points:
                        staminaAttribute.ChangeConsumeReducedPoints(_multiplyByLayer ? _deltaPoints * layer : _deltaPoints);
                        break;
                }
            }
        }
    }
}