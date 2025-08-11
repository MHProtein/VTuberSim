using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Core.RaisingEffect.RaisingEndingEffect
{
    public class VRaisingEndingEffect : VRaisingEffect
    {
        public VRaisingEndingEffect(VRaisingEffectConfiguration configuration) : base(configuration)
        {
            
        }

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);
            
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnBeginEnding, new Dictionary<string, object>()
            {
            });
        }
    }
}