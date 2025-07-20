using System.Collections.Generic;
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

        protected override void InitSetValue(int value, bool isFromCard, bool shouldPlayTwice = false)
        {
            SetValue(value, isFromCard, shouldPlayTwice);
        }

        public override void AddTo(int delta, bool isFromCard, bool shouldPlayTwice = false)
        {
        }

        protected override void SetValue(int value, bool isFromCard, bool shouldPlayTwice = false)
        {
            Value = value;
        }
    }
}