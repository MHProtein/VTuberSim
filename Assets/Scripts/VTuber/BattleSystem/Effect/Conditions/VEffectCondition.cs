using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public abstract class VEffectCondition : VScriptableObject
    {
        public abstract bool IsTrue(VBattle battle, Dictionary<string, object> message);
    }
}