using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect
{
    public class VRedrawEffect : VEffect
    {
        public VRedrawEffect(VRedrawEffectConfiguration configuration) : base(configuration)
        {
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false,
            bool shouldApplyTwice = false)
        {
            base.ApplyEffect(battle, layer, isFromCard, shouldApplyTwice);
            battle.RedrawRest();
            VDebug.Log($"效果 {_configuration.effectName} 已应用：重抽剩余卡牌。");
        }

        public override string GetValue()
        {
            return "";
        }
    }
}