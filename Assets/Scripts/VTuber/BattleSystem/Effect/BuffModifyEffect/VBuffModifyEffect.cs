using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect
{
    public class VBuffModifyEffect : VEffect
    {
        VBuffModifyEffectConfiguration _configuration;
        public VBuffModifyEffect(VBuffModifyEffectConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {            
            battle.BuffManager.ModifyBuff(_configuration.buffName, _configuration.addValue * layer, isFromCard, shouldApplyTwice);
        }
            
    }
}