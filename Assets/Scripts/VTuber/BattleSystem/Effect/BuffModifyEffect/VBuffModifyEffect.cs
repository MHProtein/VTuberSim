using System;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.BattleSystem.Effect
{
    public class VBuffModifyEffect : VEffect
    {
        VBuffModifyEffectConfiguration _configuration;
        public VUpgradableValue<int> _addValue;
 
        public VBuffModifyEffect(VBuffModifyEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _configuration = configuration;
            _addValue = new VUpgradableValue<int>(Convert.ToInt32(parameter), Convert.ToInt32(upgradedParameter));
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            int value = _addValue.Value;
            if (MultiplyByLayer > 0.0f)
                value *= (int)(layer * MultiplyByLayer);
            
            battle.BuffManager.AddBuff(VBattleDataManager.Instance.CreateBuffByID(_configuration.buffID), value, isFromCard, shouldApplyTwice);
            VDebug.Log("Effect " + _configuration.effectName + " added " + value + " to buff with ID: " + _configuration.buffID + ". New value: " + value);
        }
            
    }
}