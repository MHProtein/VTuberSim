using System.Collections.Generic;
using UnityEditor.Sprites;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Core;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VBuffLayerCondition : VEffectCondition
    {
        public int buffId;
        public int layerCount;
        
        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            if (battle.BuffManager.TryGetBuff(buffId, out var buff))
            {
                if (buff.value > layerCount)
                    return true;
            }

            return false;
        }
    }
}