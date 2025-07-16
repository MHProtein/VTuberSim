using Spire.Xls;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.BattleSystem.Effect
{
    public class VAddParamsBuffPercentageEffect : VEffect
    {        
        private readonly uint _buffID;
        private readonly VUpgradableValue<float> _percentage;
        
        public VAddParamsBuffPercentageEffect(VEffectConfiguration configuration, uint buffID, float percentage, float upgradedPercentage) : base(configuration)
        {
            _buffID = buffID;
            _percentage = new VUpgradableValue<float>(percentage, upgradedPercentage);
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            base.ApplyEffect(battle, layer, isFromCard, shouldApplyTwice);
            
            if (battle.BuffManager.TryGetBuff(_buffID, out var buff))
            {
                int delta = (int)((_percentage.Value) * buff.value);
                
                if (_configuration.multiplyByLayer > 0.0f)
                    delta *= (int)(layer * _configuration.multiplyByLayer);
                
                if (battle.BattleAttributeManager.TryGetAttribute("BAParameter", out var attribute))
                {
                    attribute.AddTo(delta, isFromCard, shouldApplyTwice);
                    VDebug.Log($"Effect{_configuration.effectName} added {delta} to BAParameter. New value: {attribute.Value}");
                }
            }
        }
    }
}