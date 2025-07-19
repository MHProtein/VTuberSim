using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleMultiplierAttribute : VBattleAttribute
    {
        public readonly Color color;
        public VBattleMultiplierAttribute(int value, Color color) : base(value, true, VBattleEventKey.OnMultiplierChange)
        {
            this.color = color;
        }
    }
}