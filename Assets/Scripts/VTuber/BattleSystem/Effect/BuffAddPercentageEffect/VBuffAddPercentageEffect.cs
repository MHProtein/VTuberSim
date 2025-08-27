using System.Collections.Generic;
using System.Globalization;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.BattleSystem.Effect
{
    public class VBuffAddPercentageEffect : VEffect
    {
        private readonly uint _buffID;
        private readonly VUpgradableValue<float> _percentage;

        public VBuffAddPercentageEffect(VEffectConfiguration configuration, uint buffID, float percentage, float upgradedPercentage) : base(configuration)
        {
            _buffID = buffID;
            _percentage = new VUpgradableValue<float>(percentage, upgradedPercentage);
        }
        
        public override void Upgrade()
        {
            base.Upgrade();
            _percentage.Upgrade();
        }
        
        public override void Downgrade()
        {
            base.Downgrade();
            _percentage.Downgrade();
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            if (battle.BuffManager.TryGetBuff(_buffID, out var buff))
            {

                int value = (int)((_percentage.Value) * buff.Value);
                
                if (_configuration.multiplyByLayer > 0.0f)
                    value *= (int)(layer * _configuration.multiplyByLayer);
                value = value == 0 ? (_percentage.Value > 0.0f ? 1 : -1) : value;
                
                battle.BuffManager.AddBuff(VDataManager.Instance.CreateBuffByID(_buffID), value, isFromCard, shouldApplyTwice);
                VDebug.Log("Effect " + _configuration.effectName + " added " + value + " to buff with ID: " + _buffID + ". New value: " + buff.Value);
            }
            else
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                    new Dictionary<string, object>());
            }
        }
        
        public override string GetValue()
        {
            return (int)(_percentage.Value * 100) + "%";
        }
    }
}