using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect
{
    public class VEffectConfiguration : VScriptableObject
    {
        [StringToEnum] public string effectName;
        [TextArea] public string description;
    }
}