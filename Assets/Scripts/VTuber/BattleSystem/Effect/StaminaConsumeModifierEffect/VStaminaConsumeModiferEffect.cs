using System;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect.StaminaConsumeModifierEffect
{
    public class VStaminaConsumeModiferEffect : VEffect
    {
        private VStaminaConsumeModifierType _modifierType;
        
        private float _deltaRate;

        private int _deltaPoints;
        
        private bool _multiplyByLayer;
        
        public VStaminaConsumeModiferEffect(VStaminaConsumeModiferEffectConfiguration configuration) : base(configuration)
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
                    case VStaminaConsumeModifierType.Rate:
                        staminaAttribute.ChangeConsumeRate(_multiplyByLayer ? _deltaRate * layer : _deltaRate);
                        break;
                    case VStaminaConsumeModifierType.Points:
                        staminaAttribute.ChangeConsumeReducedPoints(_multiplyByLayer ? _deltaPoints * layer : _deltaPoints);
                        break;
                }
            }
        }
    }
}