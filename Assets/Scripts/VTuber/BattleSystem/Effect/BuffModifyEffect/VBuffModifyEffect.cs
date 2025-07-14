using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

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
            battle.BuffManager.AddBuff(VBattleDataManager.Instance.CreateBuffByID(_configuration.buffID), _configuration.addValue * layer, isFromCard, shouldApplyTwice);
        }
            
    }
}