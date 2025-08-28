using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.Effect
{
    public class VAddEffect : VEffect, IVValuePreview
    {
        public string AttributeName => _configuration.attributeName;
        private VUpgradableValue<int> _addValue;
        VAddEffectConfiguration _configuration;
        private string _attribute;

        public VAddEffect(VAddEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _configuration = configuration;
            
            _addValue = new VUpgradableValue<int>(int.Parse(parameter), int.Parse(upgradedParameter));
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            if (battle is not null && battle.BattleAttributeManager is not null)
                return;
            if (battle.BattleAttributeManager.TryGetAttribute(_configuration.attributeName, out var attribute))
            {
                int value = _addValue.Value;
                if (MultiplyByLayer > 0.0f)
                    value *= VMathUtils.FloatToInt(layer * MultiplyByLayer);
                if (value == 0 && isFromCard)
                {
                    VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                        new Dictionary<string, object>());
                    return;
                }
                attribute.AddTo(value, isFromCard, shouldApplyTwice);
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();
            _addValue.Upgrade();
        }
        
        public override void Downgrade()
        {
            base.Downgrade();
            _addValue.Downgrade();
        }

        public override string GetValue()
        {
            return _addValue.Value.ToString();
        }

        public int GetValue(VBattle battle)
        {
            return _addValue.Value;
        }
    }
}