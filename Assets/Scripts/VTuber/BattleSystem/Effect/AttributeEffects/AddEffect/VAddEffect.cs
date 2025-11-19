using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.Effect
{
    public class VAddEffect : VEffect, IVValuePreview
    {
        private readonly VUpgradableValue<int> _addValue;
        private readonly VAddEffectConfiguration _configuration;
        private string _attribute;

        public VAddEffect(VAddEffectConfiguration configuration, string parameter, string upgradedParameter) : base(
            configuration)
        {
            _configuration = configuration;

            try
            {
                _addValue = new VUpgradableValue<int>(int.Parse(parameter), int.Parse(upgradedParameter));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string AttributeName => _configuration.attributeName;

        public int GetValue(VBattle battle)
        {
            return _addValue.Value;
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false,
            bool shouldApplyTwice = false)
        {
            base.ApplyEffect(battle, layer, isFromCard, shouldApplyTwice);
            if (battle is null || battle.BattleAttributeManager is null)
                return;
            if (battle.BattleAttributeManager.TryGetAttribute(_configuration.attributeName, out var attribute))
            {
                var value = _addValue.Value;
                if (MultiplyByLayer > 0.0f)
                    value *= VMathUtils.FloatToInt(layer * MultiplyByLayer);
                // if (value == 0 && isFromCard)
                // {
                //     VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnNotifyBeginDisposeCard,
                //         new Dictionary<string, object>());
                //     return;
                // }

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
    }
}