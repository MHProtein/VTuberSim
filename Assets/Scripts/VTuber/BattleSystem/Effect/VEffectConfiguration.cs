using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect
{
    public class VEffectConfiguration : VScriptableObject
    {
        [StringToEnum("Effects")] public string effectName;
        [TextArea] public string description;
        
        public List<VEffectCondition> conditions;
        
        public virtual VEffect CreateEffect()
        {
            return new VEffect(this);
        }

    }
}