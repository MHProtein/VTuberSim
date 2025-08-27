using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.Effect
{
    public class VAddParamAttributePercentageEffect : VEffect, IVValuePreview
    {
        public string AttributeName => "BAParameter";

        private string attributeName;
        private VUpgradableValue<float> _percentage;
        
        public VAddParamAttributePercentageEffect(VAddParamAttributePercentageEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            attributeName = configuration.attributeName;
            _percentage = new VUpgradableValue<float>(Convert.ToSingle(parameter), Convert.ToSingle(upgradedParameter));
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            base.ApplyEffect(battle, layer, isFromCard, shouldApplyTwice);
            
            if (battle.BattleAttributeManager.TryGetAttribute(attributeName, out var attribute))
            {
                int delta = VMathUtils.FloatToInt((_percentage.Value) * attribute.Value);
                
                if (MultiplyByLayer > 0.0f)
                    delta *= VMathUtils.FloatToInt(layer * MultiplyByLayer);

                if (delta == 0 && isFromCard)
                {
                    VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                        new Dictionary<string, object>());
                    return;
                }
                
                if(battle.BattleAttributeManager.TryGetAttribute("BAParameter", out var baAttribute))
                {
                    baAttribute.AddTo(delta, isFromCard, shouldApplyTwice);
                    VDebug.Log($"效果{_configuration.effectName} 为 BAParameter 增加了 {delta}。新数值: {baAttribute.Value}");
                }
                VDebug.Log($"效果{_configuration.effectName} 为 {attributeName} 增加了 {delta}。新数值: {attribute.Value}");
            }
            else
            {
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                    new Dictionary<string, object>());
            }
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

        public int GetValue(VBattle battle)
        {
            if (battle.BattleAttributeManager.TryGetAttribute(attributeName, out var attribute))
            {
                return VMathUtils.FloatToInt((_percentage.Value) * attribute.Value);
            }
            return 0;
        }
        
        public override string GetValue()
        {
            return VMathUtils.FloatToInt(_percentage.Value * 100) + "%";
        }
    }
}