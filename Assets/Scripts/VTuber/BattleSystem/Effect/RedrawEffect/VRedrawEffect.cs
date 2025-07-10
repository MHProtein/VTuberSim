using UnityEngine;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect
{
    public class VRedrawEffect : VEffect
    {
        public VRedrawEffect(VRedrawEffectConfiguration configuration) : base(configuration)
        {
            
        }

        public override void ApplyEffect(VBattle battle)
        {
            base.ApplyEffect(battle);
            battle.RedrawRest();
        }
    }
}