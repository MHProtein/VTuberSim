using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect
{
    public class VRedrawEffect : VEffect
    {
        public VRedrawEffect(VRedrawEffectConfiguration configuration) : base(configuration)
        {
            
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            battle.RedrawRest();
            VDebug.Log($"Effect {_configuration.effectName} applied. Redrawing rest of the cards.");
        }
    }
}