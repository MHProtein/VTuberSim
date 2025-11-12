using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect
{
    public class VDrawCardEffect : VEffect
    {
        private readonly VUpgradableValue<int> _drawCardCount;

        public VDrawCardEffect(VDrawCardEffectConfiguration configuration, string parameter, string upgradedParameter) :
            base(configuration)
        {
            try
            {
                _drawCardCount =
                    new VUpgradableValue<int>(Convert.ToInt32(parameter), Convert.ToInt32(upgradedParameter));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false,
            bool shouldPlayTwice = false)
        {
            base.ApplyEffect(battle, layer, isFromCard, shouldPlayTwice);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRequestDrawCards, new Dictionary<string, object>
            {
                { "DrawCount", _drawCardCount.Value },
                { "IsFromCard", isFromCard },
                { "ShouldPlayTwice", shouldPlayTwice }
            });
            VDebug.Log($"效果 {_configuration.effectName} 请求抽取 {_drawCardCount.Value} 张卡牌。");
        }

        public override void Upgrade()
        {
            base.Upgrade();
            _drawCardCount.Upgrade();
        }

        public override void Downgrade()
        {
            base.Downgrade();
            _drawCardCount.Downgrade();
        }

        public override string GetValue()
        {
            return _drawCardCount.Value.ToString();
        }
    }
}