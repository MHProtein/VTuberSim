using System.Collections.Generic;
using System.Globalization;
using Spire.Xls;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.BattleSystem.Effect
{
    public class VAddParamsBuffPercentageEffect : VEffect, IVValuePreview
    {        
        private readonly uint _buffID;
        private readonly VUpgradableValue<float> _percentage;
        
        public VAddParamsBuffPercentageEffect(VEffectConfiguration configuration, uint buffID, float percentage, float upgradedPercentage) : base(configuration)
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
            base.ApplyEffect(battle, layer, isFromCard, shouldApplyTwice);
            
            if (battle.BuffManager.TryGetBuff(_buffID, out var buff))
            {
                int delta = (int)((_percentage.Value) * buff.Value);
                
                if (MultiplyByLayer > 0.0f)
                    delta *= (int)(layer * MultiplyByLayer);
                
                if (battle.BattleAttributeManager.TryGetAttribute("BAParameter", out var attribute))
                {
                    attribute.AddTo(delta, isFromCard, shouldApplyTwice);
                    VDebug.Log($"效果{_configuration.effectName} 为 BAParameter 增加了 {delta}。新数值: {attribute.Value}");
                }
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

        public int GetValue(VBattle battle)
        {
            if (battle.BuffManager.TryGetBuff(_buffID, out var buff))
            {
                return (int)((_percentage.Value) * buff.Value);
            }
            return 0;
        }
    }
}